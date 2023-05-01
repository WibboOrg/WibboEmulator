namespace WibboEmulator.Games.Chats.Commands.Staff.Animation;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Moderations;
using WibboEmulator.Games.Rooms;

internal sealed class GiveLot : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser user, string[] parts)
    {
        if (parts.Length != 2)
        {
            return;
        }

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByName(parts[1]);
        if (roomUserByUserId == null || roomUserByUserId.Client == null)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
            return;
        }
        if (roomUserByUserId.GetUsername() == session.User.Username || roomUserByUserId.Client.User.IP == session.User.IP)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.givelot.error", session.Langue));
            ModerationManager.LogStaffEntry(session.User.Id, session.User.Username, 0, string.Empty, "notallowed", "Tentative de GiveLot: " + roomUserByUserId.GetUsername());
            return;
        }

        int lotCount;

        if (user.Client.User.HasPermission("premium_legend"))
        {
            lotCount = 5;
        }
        else if (user.Client.User.HasPermission("premium_epic"))
        {
            lotCount = WibboEnvironment.GetRandomNumber(3, 5);
        }
        else if (user.Client.User.HasPermission("premium_classic"))
        {
            lotCount = WibboEnvironment.GetRandomNumber(2, 3);
        }
        else
        {
            lotCount = WibboEnvironment.GetRandomNumber(1, 2);
        }

        var lootboxId = WibboEnvironment.GetSettings().GetData<int>("givelot.lootbox.id");

        if (!WibboEnvironment.GetGame().GetItemManager().GetItem(lootboxId, out var itemData))
        {
            return;
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();

        var items = ItemFactory.CreateMultipleItems(dbClient, itemData, roomUserByUserId.Client.User, "", lotCount);

        foreach (var purchasedItem in items)
        {
            roomUserByUserId.Client.User.InventoryComponent.TryAddItem(purchasedItem);
        }

        roomUserByUserId.Client.SendNotification(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("notif.givelot.sucess", roomUserByUserId.Client.Langue), lotCount));
        session.SendWhisper(roomUserByUserId.GetUsername() + " à reçu " + lotCount + " RareBox!");

        UserDao.UpdateAddGamePoints(dbClient, roomUserByUserId.Client.User.Id);

        _ = WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(roomUserByUserId.Client, "ACH_Extrabox", 1);
    }
}
