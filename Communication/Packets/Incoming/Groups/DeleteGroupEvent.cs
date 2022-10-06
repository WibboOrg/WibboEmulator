namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Database.Daos.Guild;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

internal class DeleteGroupEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var groupId = Packet.PopInt();

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out var Group))
        {
            return;
        }

        if (Group.CreatorId != session.GetUser().Id && !session.GetUser().HasPermission("perm_delete_group"))
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.groupdelete.error.1", session.Langue));
            return;
        }

        if (Group.MemberCount >= 100 && !session.GetUser().HasPermission("perm_delete_group_limit"))
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.groupdelete.error.2", session.Langue));
            return;
        }

        WibboEnvironment.GetGame().GetGroupManager().DeleteGroup(Group.Id);

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            GuildDao.Delete(dbClient, Group.Id);
            GuildMembershipDao.Delete(dbClient, Group.Id);
            GuildRequestDao.Delete(dbClient, Group.Id);
            RoomDao.UpdateResetGroupId(dbClient, Group.RoomId);
            UserStatsDao.UpdateRemoveAllGroupId(dbClient, Group.Id);

            if (Group.CreatorId != session.GetUser().Id)
            {
                LogStaffDao.Insert(dbClient, session.GetUser().Username, $"Suppresion du groupe {Group.Id} crée par {Group.CreatorId}");
            }
        }

        session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.groupdelete.succes", session.Langue));

        if (WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out var room))
        {
            room.RoomData.Group = null;
            WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(room);
        }
    }
}
