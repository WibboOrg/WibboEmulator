using Butterfly.Game.GameClients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class KickUserEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
            {
                return;
            }

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || room.RoomData.WhoCanKick != 2 && (room.RoomData.WhoCanKick != 1 || !room.CheckRights(Session)) && !room.CheckRights(Session, true) && Session.GetHabbo().Rank < 6)
            {
                return;
            }

            int pId = Packet.PopInt();

            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(pId);
            if (roomUserByHabbo == null || roomUserByHabbo.IsBot || (room.CheckRights(roomUserByHabbo.GetClient(), true) || roomUserByHabbo.GetClient().GetHabbo().HasFuse("fuse_mod")) || roomUserByHabbo.GetClient().GetHabbo().HasFuse("fuse_no_kick"))
            {
                return;
            }

            room.GetRoomUserManager().RemoveUserFromRoom(roomUserByHabbo.GetClient(), true, true);
        }
    }
}