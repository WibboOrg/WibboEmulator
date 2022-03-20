using Butterfly.Communication.Packets.Outgoing.Camera;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class PurchasePhotoEvent : IPacketEvent
    {
        public double Delay => 200;

        public void Parse(Client session, ClientPacket packet)
        {
            string photoId = packet.PopString();

            if (string.IsNullOrEmpty(photoId))
            {
                photoId = session.GetUser().LastPhotoId;
            }
            else if(!ButterflyEnvironment.IsValidAlphaNumeric(photoId) || photoId.Length != 32)
            {
                session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.buyphoto.error", session.Langue) + " ( " + photoId + " ) ");
                return;
            }

            if (string.IsNullOrEmpty(photoId))
            {
                session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.buyphoto.error", session.Langue) + " ( " + photoId + " ) ");
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
            string ExtraData = "{\"w\":\"" + "/photos/" + photoId + ".png" + "\", \"n\":\"" + session.GetUser().Username + "\", \"s\":\"" + session.GetUser().Id + "\", \"u\":\"" + "0" + "\", \"t\":\"" + Time + "000" + "\"}";

            Item ItemSmall = ItemFactory.CreateSingleItemNullable(ItemDataSmall, session.GetUser(), ExtraData);
            session.GetUser().GetInventoryComponent().TryAddItem(ItemSmall);

            Item Item = ItemFactory.CreateSingleItemNullable(ItemData, session.GetUser(), ExtraData);
            session.GetUser().GetInventoryComponent().TryAddItem(Item);

            session.SendPacket(new CameraPurchaseSuccesfullComposer());
        }
    }
}
