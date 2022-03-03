using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Guilds;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class DeleteGroupEvent : IPacketEvent
    {
        public double Delay => 1000;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int groupId = Packet.PopInt();

            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out Group Group))
            {
                return;
            }

            if (Group.CreatorId != Session.GetUser().Id && !Session.GetUser().HasFuse("group_delete_override"))
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.groupdelete.error.1", Session.Langue));
                return;
            }

            if (Group.MemberCount >= 100 && !Session.GetUser().HasFuse("group_delete_limit_override"))
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.groupdelete.error.2", Session.Langue));
                return;
            }

            Room Room = ButterflyEnvironment.GetGame().GetRoomManager().LoadRoom(Group.RoomId);

            if (Room != null)
            {
                Room.RoomData.Group = null;
            }

            ButterflyEnvironment.GetGame().GetGroupManager().DeleteGroup(Group.Id);

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
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

            ButterflyEnvironment.GetGame().GetRoomManager().UnloadRoom(Room);

            Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.groupdelete.succes", Session.Langue));
        }
    }
}
