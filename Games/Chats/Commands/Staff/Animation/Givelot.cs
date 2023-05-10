namespace WibboEmulator.Games.Chats.Commands.Staff.Animation;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Moderations;
using WibboEmulator.Games.Rooms;

internal sealed class GiveLot : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parts)
    {
        if (parts.Length != 2)
        {
            return;
        }

        var targetRoomUser = room.RoomUserManager.GetRoomUserByName(parts[1]);
        if (targetRoomUser == null || targetRoomUser.Client == null)
        {
            userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
            return;
        }
        // if (targetRoomUser.GetUsername() == session.User.Username || targetRoomUser.Client.User.IP == session.User.IP)
        // {
        //     userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("notif.givelot.error", session.Langue));
        //     ModerationManager.LogStaffEntry(session.User.Id, session.User.Username, 0, string.Empty, "notallowed", "Tentative de GiveLot: " + targetRoomUser.GetUsername());
        //     return;
        // }

        int lotCount;

        if (targetRoomUser.Client.User.HasPermission("premium_legend"))
        {
            lotCount = 5;
        }
        else if (targetRoomUser.Client.User.HasPermission("premium_epic"))
        {
            lotCount = WibboEnvironment.GetRandomNumber(3, 5);
        }
        else if (targetRoomUser.Client.User.HasPermission("premium_classic"))
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

        var items = ItemFactory.CreateMultipleItems(dbClient, itemData, targetRoomUser.Client.User, "", lotCount);

        foreach (var purchasedItem in items)
        {
            targetRoomUser.Client.User.InventoryComponent.TryAddItem(purchasedItem);
        }

        targetRoomUser.Client.SendNotification(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("notif.givelot.sucess", targetRoomUser.Client.Langue), lotCount));
        session.SendWhisper(targetRoomUser.GetUsername() + " à reçu " + lotCount + " RareBox!");

        targetRoomUser.Client.User.GamePointsMonth += 1;
        WibboEnvironment.GetGame().GetHallOFFame().UpdateRakings(targetRoomUser.Client.User);
        UserDao.UpdateAddGamePoints(dbClient, targetRoomUser.Client.User.Id);

        _ = WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(targetRoomUser.Client, "ACH_Extrabox", 1);
    }
}
