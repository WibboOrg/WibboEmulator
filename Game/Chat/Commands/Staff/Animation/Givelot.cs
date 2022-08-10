using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Items;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class GiveLot : IChatCommand
    {
        public void Execute(Client session, Room room, RoomUser user, string[] parts)
        {
            if (parts.Length != 2)
            {
                return;
            }

            RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByName(parts[1]);
            if (roomUserByUserId == null || roomUserByUserId.GetClient() == null)
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
                return;
            }
            if (roomUserByUserId.GetUsername() == session.GetUser().Username || roomUserByUserId.GetClient().GetUser().IP == session.GetUser().IP)
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.givelot.error", session.Langue));
                WibboEnvironment.GetGame().GetModerationManager().LogStaffEntry(session.GetUser().Id, session.GetUser().Username, 0, string.Empty, "notallowed", "Tentative de GiveLot: " + roomUserByUserId.GetUsername());
                return;
            }

            int lotCount = WibboEnvironment.GetRandomNumber(1, 2);
            if (roomUserByUserId.GetClient().GetUser().Rank > 1)
            {
                lotCount = WibboEnvironment.GetRandomNumber(2, 3);
            }

            if (!WibboEnvironment.GetGame().GetItemManager().GetItem(73917766, out ItemData ItemData))
            {
                return;
            }

            List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, roomUserByUserId.GetClient().GetUser(), "", lotCount);

            foreach (Item PurchasedItem in Items)
            {
                if (roomUserByUserId.GetClient().GetUser().GetInventoryComponent().TryAddItem(PurchasedItem))
                {
                    roomUserByUserId.GetClient().SendPacket(new FurniListNotificationComposer(PurchasedItem.Id, 1));
                }
            }

            roomUserByUserId.GetClient().SendNotification(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("notif.givelot.sucess", roomUserByUserId.GetClient().Langue), lotCount, NbBadge) + DeluxeMessage);
            session.SendWhisper(roomUserByUserId.GetUsername() + " à reçu " + lotCount + " RareBox!");

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateAddGamePoints(dbClient, roomUserByUserId.GetClient().GetUser().Id);
            }

            WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(roomUserByUserId.GetClient(), "ACH_Extrabox", 1);
        }
    }
}
