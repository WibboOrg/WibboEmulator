using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Groups;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class DeleteGroupEvent : IPacketEvent
    {
        public double Delay => 1000;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int groupId = Packet.PopInt();

            if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out Group Group))
            {
                return;
            }

            if (Group.CreatorId != Session.GetUser().Id && !Session.GetUser().HasPermission("perm_delete_group"))
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.groupdelete.error.1", Session.Langue));
                return;
            }

            if (Group.MemberCount >= 100 && !Session.GetUser().HasPermission("perm_delete_group_limit"))
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.groupdelete.error.2", Session.Langue));
                return;
            }

            WibboEnvironment.GetGame().GetGroupManager().DeleteGroup(Group.Id);

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                GuildDao.Delete(dbClient, Group.Id);
                GuildMembershipDao.Delete(dbClient, Group.Id);
                GuildRequestDao.Delete(dbClient, Group.Id);
                RoomDao.UpdateResetGroupId(dbClient, Group.RoomId);
                UserStatsDao.UpdateRemoveAllGroupId(dbClient, Group.Id);

                if (Group.CreatorId != Session.GetUser().Id)
                {
                    LogStaffDao.Insert(dbClient, Session.GetUser().Username, $"Suppresion du groupe {Group.Id} crée par {Group.CreatorId}");
                }
            }

            Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.groupdelete.succes", Session.Langue));

            if (WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out Room room))
            {
                room.RoomData.Group = null;
                WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(room);
            }
        }
    }
}
