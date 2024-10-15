namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;

using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Rooms;

internal sealed class DeleteGroup : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        var groupId = int.TryParse(parameters[1], out var id) ? id : 0;

        if (!GroupManager.TryGetGroup(groupId, out var group))
        {
            return;
        }

        if (group.CreatorId != Session.User.Id && !Session.User.HasPermission("delete_group"))
        {
            Session.SendNotification(LanguageManager.TryGetValue("notif.groupdelete.error.1", Session.Language));
            return;
        }

        if (group.MemberCount >= 100 && !Session.User.HasPermission("delete_group_limit"))
        {
            Session.SendNotification(LanguageManager.TryGetValue("notif.groupdelete.error.2", Session.Language));
            return;
        }

        GroupManager.DeleteGroup(group.Id);

        using var dbClient = DatabaseManager.Connection;

        if (group.CreatorId != Session.User.Id)
        {
            LogStaffDao.Insert(dbClient, Session.User.Username, $"Suppresion du groupe {group.Id} crée par {group.CreatorId}");
        }

        Session.SendNotification(LanguageManager.TryGetValue("notif.groupdelete.succes", Session.Language));

        if (RoomManager.TryGetRoom(group.RoomId, out var roomGroup))
        {
            RoomManager.UnloadRoom(roomGroup);
        }

    }
}
