namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;

using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Rooms;

internal sealed class DeleteGroup : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var groupId = int.TryParse(parameters[1], out var id) ? id : 0;

        if (!GroupManager.TryGetGroup(groupId, out var group))
        {
            return;
        }

        if (group.CreatorId != session.User.Id && !session.User.HasPermission("delete_group"))
        {
            session.SendNotification(LanguageManager.TryGetValue("notif.groupdelete.error.1", session.Language));
            return;
        }

        if (group.MemberCount >= 100 && !session.User.HasPermission("delete_group_limit"))
        {
            session.SendNotification(LanguageManager.TryGetValue("notif.groupdelete.error.2", session.Language));
            return;
        }

        GroupManager.DeleteGroup(group.Id);

        using var dbClient = DatabaseManager.Connection;

        if (group.CreatorId != session.User.Id)
        {
            LogStaffDao.Insert(dbClient, session.User.Username, $"Suppresion du groupe {group.Id} crée par {group.CreatorId}");
        }

        session.SendNotification(LanguageManager.TryGetValue("notif.groupdelete.succes", session.Language));

        if (RoomManager.TryGetRoom(group.RoomId, out var roomGroup))
        {
            RoomManager.UnloadRoom(roomGroup);
        }

    }
}
