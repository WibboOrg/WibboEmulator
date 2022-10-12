namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Database.Daos.Guild;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

internal class DeleteGroupEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var groupId = packet.PopInt();

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out var group))
        {
            return;
        }

        if (group.CreatorId != session.GetUser().Id && !session.GetUser().HasPermission("perm_delete_group"))
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.groupdelete.error.1", session.Langue));
            return;
        }

        if (group.MemberCount >= 100 && !session.GetUser().HasPermission("perm_delete_group_limit"))
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.groupdelete.error.2", session.Langue));
            return;
        }

        WibboEnvironment.GetGame().GetGroupManager().DeleteGroup(group.Id);

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            GuildDao.Delete(dbClient, group.Id);
            GuildMembershipDao.Delete(dbClient, group.Id);
            GuildRequestDao.Delete(dbClient, group.Id);
            RoomDao.UpdateResetGroupId(dbClient, group.RoomId);
            UserStatsDao.UpdateRemoveAllGroupId(dbClient, group.Id);

            if (group.CreatorId != session.GetUser().Id)
            {
                LogStaffDao.Insert(dbClient, session.GetUser().Username, $"Suppresion du groupe {group.Id} crée par {group.CreatorId}");
            }
        }

        session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.groupdelete.succes", session.Langue));

        if (WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(group.RoomId, out var room))
        {
            room.Data.Group = null;
            WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(room);
        }
    }
}
