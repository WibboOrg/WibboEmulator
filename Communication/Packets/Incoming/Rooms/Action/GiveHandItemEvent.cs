using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GiveHandItemEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetUser() == null)
            {
                return;
            }

            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            if (roomUserByUserId == null || roomUserByUserId.CarryItemID <= 0 || roomUserByUserId.CarryTimer <= 0)
            {
                return;
            }

            RoomUser roomUserByUserIdTarget = room.GetRoomUserManager().GetRoomUserByUserId(Packet.PopInt());
            if (roomUserByUserIdTarget == null)
            {
                return;
            }

            if (Math.Abs(roomUserByUserId.X - roomUserByUserIdTarget.X) >= 3 || Math.Abs(roomUserByUserId.Y - roomUserByUserIdTarget.Y) >= 3)
            {
                roomUserByUserId.MoveTo(roomUserByUserIdTarget.X, roomUserByUserIdTarget.Y);
                return;
            }

            roomUserByUserIdTarget.CarryItem(roomUserByUserId.CarryItemID);
            roomUserByUserId.CarryItem(0);
        }
    }
}