using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class KickUserEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetUser() == null)
            {
                return;
            }

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null || room.RoomData.WhoCanKick != 2 && (room.RoomData.WhoCanKick != 1 || !room.CheckRights(Session)) && !room.CheckRights(Session, true) && Session.GetUser().Rank < 6)
            {
                return;
            }

            int pId = Packet.PopInt();

            RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(pId);
            if (roomUserByUserId == null || roomUserByUserId.IsBot || (room.CheckRights(roomUserByUserId.GetClient(), true) || roomUserByUserId.GetClient().GetUser().HasFuse("fuse_mod")) || roomUserByUserId.GetClient().GetUser().HasFuse("fuse_no_kick"))
            {
                return;
            }

            room.GetRoomUserManager().RemoveUserFromRoom(roomUserByUserId.GetClient(), true, true);
        }
    }
}