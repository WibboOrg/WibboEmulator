namespace WibboEmulator.Games.Chats.Commands.Staff.Animation;

using WibboEmulator.Core.Language;
using WibboEmulator.Core.Settings;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Achievements;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.LandingView;
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
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("input.usernotfound", session.Language));
            return;
        }

        if (targetRoomUser.Username == session.User.Username || targetRoomUser.Client.User.IP == session.User.IP)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("notif.givelot.error", session.Language));
            ModerationManager.LogStaffEntry(session.User.Id, session.User.Username, 0, string.Empty, "notallowed", "Tentative de GiveLot: " + targetRoomUser.Username);
            return;
        }

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

        var lootboxId = SettingsManager.GetData<int>("givelot.lootbox.id");

        if (!ItemManager.GetItem(lootboxId, out var itemData))
        {
            return;
        }

        using var dbClient = DatabaseManager.Connection;

        var items = ItemFactory.CreateMultipleItems(dbClient, itemData, targetRoomUser.Client.User, "", lotCount);

        foreach (var purchasedItem in items)
        {
            targetRoomUser.Client.User.InventoryComponent.TryAddItem(purchasedItem);
        }

        targetRoomUser.Client.SendNotification(string.Format(LanguageManager.TryGetValue("notif.givelot.sucess", targetRoomUser.Client.Language), lotCount));
        session.SendWhisper(targetRoomUser.Username + " à reçu " + lotCount + " LootBox!");

        targetRoomUser.Client.User.GamePointsMonth += 1;
        HallOfFameManager.UpdateRakings(targetRoomUser.Client.User);
        UserDao.UpdateAddGamePoints(dbClient, targetRoomUser.Client.User.Id);

        _ = AchievementManager.ProgressAchievement(targetRoomUser.Client, "ACH_Extrabox", 1);
    }
}
