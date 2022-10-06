namespace WibboEmulator.Games.Chat.Commands.Staff.Animation;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal class GiveLot : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser user, string[] parts)
    {
        if (parts.Length != 2)
        {
            return;
        }

        var roomUserByUserId = room.GetRoomUserManager().GetRoomUserByName(parts[1]);
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

        var lotCount = WibboEnvironment.GetRandomNumber(1, 2);
        if (roomUserByUserId.GetClient().GetUser().Rank > 1)
        {
            lotCount = WibboEnvironment.GetRandomNumber(2, 3);
        }

        var lootboxId = WibboEnvironment.GetSettings().GetData<int>("givelot.lootbox.id");

        if (!WibboEnvironment.GetGame().GetItemManager().GetItem(lootboxId, out var ItemData))
        {
            return;
        }

        var Items = ItemFactory.CreateMultipleItems(ItemData, roomUserByUserId.GetClient().GetUser(), "", lotCount);

        foreach (var PurchasedItem in Items)
        {
            if (roomUserByUserId.GetClient().GetUser().GetInventoryComponent().TryAddItem(PurchasedItem))
            {
                roomUserByUserId.GetClient().SendPacket(new FurniListNotificationComposer(PurchasedItem.Id, 1));
            }
        }

        roomUserByUserId.GetClient().SendNotification(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("notif.givelot.sucess", roomUserByUserId.GetClient().Langue), lotCount));
        session.SendWhisper(roomUserByUserId.GetUsername() + " à reçu " + lotCount + " RareBox!");

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserDao.UpdateAddGamePoints(dbClient, roomUserByUserId.GetClient().GetUser().Id);
        }

        WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(roomUserByUserId.GetClient(), "ACH_Extrabox", 1);
    }
}
