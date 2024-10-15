namespace WibboEmulator.Games.Chats.Commands.Staff.Animation;

using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Badges;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RoomBadge : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var badgeCode = parameters[1];

        if (!BadgeManager.CanGiveBadge(badgeCode))
        {
            userRoom.SendWhisperChat("Action non autorisé");
            return;
        }

        var userIds = new List<int>();

        foreach (var user in room.RoomUserManager.UserList.ToList())
        {
            if (!user.IsBot && user.Client != null && user.Client.User != null && user.Client.User.BadgeComponent != null)
            {
                user.Client.User.BadgeComponent.GiveBadge(badgeCode, false);

                userIds.Add(user.Client.User.Id);
            }
        }

        using var dbClient = DatabaseManager.Connection;
        UserBadgeDao.InsertAll(dbClient, userIds, badgeCode);
    }
}
