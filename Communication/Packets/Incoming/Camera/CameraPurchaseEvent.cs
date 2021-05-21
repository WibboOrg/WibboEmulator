using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class CameraPurchaseEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string PhotoId = Packet.PopString();

            if (string.IsNullOrEmpty(PhotoId) || !ButterflyEnvironment.IsValidAlphaNumeric(PhotoId) || PhotoId.Length != 32)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.buyphoto.error", Session.Langue) + " ( " + PhotoId + " ) ");
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetItemManager().GetItem(4581, out ItemData ItemData))
            {
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetItemManager().GetItem(4597, out ItemData ItemDataSmall))
            {
                return;
            }

            int Time = ButterflyEnvironment.GetUnixTimestamp();
            string ExtraData = "{\"w\":\"" + "/photos/" + PhotoId + ".png" + "\", \"n\":\"" + Session.GetHabbo().Username + "\", \"s\":\"" + Session.GetHabbo().Id + "\", \"u\":\"" + "0" + "\", \"t\":\"" + Time + "000" + "\"}";


            Item ItemSmall = ItemFactory.CreateSingleItemNullable(ItemDataSmall, Session.GetHabbo(), ExtraData);
            Session.GetHabbo().GetInventoryComponent().TryAddItem(ItemSmall);

            Item Item = ItemFactory.CreateSingleItemNullable(ItemData, Session.GetHabbo(), ExtraData);
            Session.GetHabbo().GetInventoryComponent().TryAddItem(Item);

            Session.SendPacket(new CameraPurchaseSuccesfullComposer());

            if (Session.GetHabbo().LastPhotoId == PhotoId)
            {
                return;
            }

            Session.GetHabbo().LastPhotoId = PhotoId;

            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("INSERT INTO user_photos (user_id,photo,time) VALUES ('" + Session.GetHabbo().Id + "', @photoid, '" + Time + "');");
                queryreactor.AddParameter("photoid", PhotoId);
                queryreactor.RunQuery();
            }
        }
    }
}
