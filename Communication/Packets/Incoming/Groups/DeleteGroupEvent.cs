using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Groups;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class DeleteGroupEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(Packet.PopInt(), out Group Group))
            {
                return;
            }

            if (Group.CreatorId != Session.GetHabbo().Id && !Session.GetHabbo().HasFuse("group_delete_override"))//Maybe a FUSE check for staff override?
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.groupdelete.error.1", Session.Langue));
                return;
            }

            if (Group.MemberCount >= 500 && !Session.GetHabbo().HasFuse("group_delete_limit_override"))
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
                RoomDao.UpdateResetGroupId(dbClient, Group.Id);
                UserStatsDao.UpdateRemoveAllGroupId(dbClient, Group.Id);
            }

            ButterflyEnvironment.GetGame().GetRoomManager().UnloadRoom(Room);

            Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.groupdelete.succes", Session.Langue));
            return;
        }
    }
}
