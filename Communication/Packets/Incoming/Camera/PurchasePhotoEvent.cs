using WibboEmulator.Communication.Packets.Outgoing.Camera;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

namespace WibboEmulator.Communication.Packets.Incoming.Camera
{
    internal class PurchasePhotoEvent : IPacketEvent
    {
        public double Delay => 200;

        public void Parse(GameClient session, ClientPacket packet)
        {
            string photoId = packet.PopString();

            if (string.IsNullOrEmpty(photoId))
            {
                photoId = session.GetUser().LastPhotoId;
            }
            else if (!WibboEnvironment.IsValidAlphaNumeric(photoId) || photoId.Length != 32)
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyphoto.error", session.Langue) + " ( " + photoId + " ) ");
                return;
            }

            if (string.IsNullOrEmpty(photoId))
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyphoto.error", session.Langue) + " ( " + photoId + " ) ");
                return;
            }

            int photoItemId = WibboEnvironment.GetSettings().GetData<int>("photo.item.id");
            if (!WibboEnvironment.GetGame().GetItemManager().GetItem(photoItemId, out ItemData ItemData))
            {
                return;
            }

            int photoSmallItemId = WibboEnvironment.GetSettings().GetData<int>("photo.small.item.id");
            if (!WibboEnvironment.GetGame().GetItemManager().GetItem(photoSmallItemId, out ItemData ItemDataSmall))
            {
                return;
            }

            int Time = WibboEnvironment.GetUnixTimestamp();
            string ExtraData = "{\"w\":\"" + "/photos/" + photoId + ".png" + "\", \"n\":\"" + session.GetUser().Username + "\", \"s\":\"" + session.GetUser().Id + "\", \"u\":\"" + "0" + "\", \"t\":\"" + Time + "000" + "\"}";

            Item ItemSmall = ItemFactory.CreateSingleItemNullable(ItemDataSmall, session.GetUser(), ExtraData);
            session.GetUser().GetInventoryComponent().TryAddItem(ItemSmall);

            Item Item = ItemFactory.CreateSingleItemNullable(ItemData, session.GetUser(), ExtraData);
            session.GetUser().GetInventoryComponent().TryAddItem(Item);

            session.SendPacket(new CameraPurchaseSuccesfullComposer());
        }
    }
}
