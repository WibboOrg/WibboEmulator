using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class MuteUserEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetUser() == null)
            {
                return;
            }

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null || (room.RoomData.MuteFuse != 1 || !room.CheckRights(Session)) && !room.CheckRights(Session, true))
            {
                return;
            }

            int Id = Packet.PopInt();
            int num = Packet.PopInt();

            RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(Id);

            int Time = Packet.PopInt() * 60;

            if (roomUserByUserId == null || roomUserByUserId.IsBot || (room.CheckRights(roomUserByUserId.GetClient(), true) || roomUserByUserId.GetClient().GetUser().HasFuse("fuse_mod") || roomUserByUserId.GetClient().GetUser().HasFuse("fuse_no_mute")))
            {
                return;
            }

            room.AddMute(Id, Time);
        }
    }
}
