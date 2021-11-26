using Butterfly.Communication.Packets.Outgoing.Camera;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class CameraPurchaseEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            string photoId = Packet.PopString();

            if (string.IsNullOrEmpty(photoId) || !ButterflyEnvironment.IsValidAlphaNumeric(photoId) || photoId.Length != 32)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.buyphoto.error", Session.Langue) + " ( " + photoId + " ) ");
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
            string ExtraData = "{\"w\":\"" + "/photos/" + photoId + ".png" + "\", \"n\":\"" + Session.GetHabbo().Username + "\", \"s\":\"" + Session.GetHabbo().Id + "\", \"u\":\"" + "0" + "\", \"t\":\"" + Time + "000" + "\"}";


            Item ItemSmall = ItemFactory.CreateSingleItemNullable(ItemDataSmall, Session.GetHabbo(), ExtraData);
            Session.GetHabbo().GetInventoryComponent().TryAddItem(ItemSmall);

            Item Item = ItemFactory.CreateSingleItemNullable(ItemData, Session.GetHabbo(), ExtraData);
            Session.GetHabbo().GetInventoryComponent().TryAddItem(Item);

            Session.SendPacket(new CameraPurchaseSuccesfullComposer());

            if (Session.GetHabbo().LastPhotoId == photoId)
            {
                return;
            }

            Session.GetHabbo().LastPhotoId = photoId;

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                UserPhotoDao.Insert(dbClient, Session.GetHabbo().Id, photoId, Time);
        }
    }
}
