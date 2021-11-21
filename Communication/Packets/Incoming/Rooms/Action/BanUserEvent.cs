using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class BanUserEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
            {
                return;
            }

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || (room.RoomData.BanFuse != 1 || !room.CheckRights(Session)) && !room.CheckRights(Session, true))
            {
                return;
            }

            int pId = Packet.PopInt();
            int num = Packet.PopInt();
            string str = Packet.PopString();

            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(pId);
            int Time;
            if (str.Equals("RWUAM_BAN_USER_HOUR"))
            {
                Time = 3600;
            }
            else if (str.Equals("RWUAM_BAN_USER_DAY"))
            {
                Time = 86400;
            }
            else
            {
                if (!str.Equals("RWUAM_BAN_USER_PERM"))
                {
                    return;
                }

                Time = 429496729;
            }
            if (roomUserByHabbo == null || roomUserByHabbo.IsBot || (room.CheckRights(roomUserByHabbo.GetClient(), true) || roomUserByHabbo.GetClient().GetHabbo().HasFuse("fuse_kickban")))
            {
                return;
            }

            room.AddBan(pId, Time);
            room.GetRoomUserManager().RemoveUserFromRoom(roomUserByHabbo.GetClient(), true, true);
        }
    }
}