namespace WibboEmulator.Games.Chats.Commands.Staff.Animation;

using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RoomBadge : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var badgeId = parameters[1];

        var userIds = new List<int>();

        foreach (var user in room.RoomUserManager.GetUserList().ToList())
        {
            if (!user.IsBot && user.Client != null && user.Client.User != null && user.Client.User.BadgeComponent != null)
            {
                user.Client.User.BadgeComponent.GiveBadge(badgeId, false);

                userIds.Add(user.Client.User.Id);
            }
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().Connection();
        UserBadgeDao.InsertAll(dbClient, userIds, badgeId);
    }
}
