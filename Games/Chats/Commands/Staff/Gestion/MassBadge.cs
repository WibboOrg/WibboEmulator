namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;

using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Badges;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class MassBadge : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var badgeCode = parameters[1];

        if (string.IsNullOrEmpty(badgeCode))
        {
            return;
        }

        if (!BadgeManager.CanGiveBadge(badgeCode))
        {
            userRoom.SendWhisperChat("Action non autorisé");
            return;
        }

        var userIds = new List<int>();

        foreach (var client in GameClientManager.Clients.ToList())
        {
            if (client.User != null)
            {
                client.User.BadgeComponent.GiveBadge(badgeCode, false);
                client.SendNotification($"Vous venez de recevoir le badge : {badgeCode} !");

                userIds.Add(client.User.Id);
            }
        }

        using var dbClient = DatabaseManager.Connection;
        UserBadgeDao.InsertAll(dbClient, userIds, badgeCode);
    }
}
