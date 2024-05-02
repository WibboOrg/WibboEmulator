namespace WibboEmulator.Communication.Packets.Incoming.Groups;

using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Guild;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Rooms;

internal sealed class DeleteGroupEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var groupId = packet.PopInt();

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

        using (var dbClient = DatabaseManager.Connection)
        {
            GuildDao.Delete(dbClient, group.Id);
            GuildMembershipDao.Delete(dbClient, group.Id);
            GuildRequestDao.Delete(dbClient, group.Id);
            RoomDao.UpdateResetGroupId(dbClient, group.RoomId);
            UserStatsDao.UpdateRemoveAllGroupId(dbClient, group.Id);

            if (group.CreatorId != session.User.Id)
            {
                LogStaffDao.Insert(dbClient, session.User.Username, $"Suppresion du groupe {group.Id} crée par {group.CreatorId}");
            }
        }

        session.SendNotification(LanguageManager.TryGetValue("notif.groupdelete.succes", session.Language));

        if (RoomManager.TryGetRoom(group.RoomId, out var room))
        {
            room.RoomData.Group = null;
            RoomManager.UnloadRoom(room);
        }
    }
}
