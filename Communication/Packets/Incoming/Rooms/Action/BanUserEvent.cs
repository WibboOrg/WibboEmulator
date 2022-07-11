using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class BanUserEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetUser() == null)
            {
                return;
            }

            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null || (room.RoomData.BanFuse != 1 || !room.CheckRights(Session)) && !room.CheckRights(Session, true))
            {
                return;
            }

            int pId = Packet.PopInt();
            int num = Packet.PopInt();
            string str = Packet.PopString();

            RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(pId);
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
            if (roomUserByUserId == null || roomUserByUserId.IsBot || (room.CheckRights(roomUserByUserId.GetClient(), true) || roomUserByUserId.GetClient().GetUser().HasFuse("fuse_kickban")))
            {
                return;
            }

            room.AddBan(pId, Time);
            room.GetRoomUserManager().RemoveUserFromRoom(roomUserByUserId.GetClient(), true, true);
        }
    }
}