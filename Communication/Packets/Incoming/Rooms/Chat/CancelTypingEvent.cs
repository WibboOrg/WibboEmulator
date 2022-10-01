using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class CancelTypingEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            if (roomUserByUserId == null)
            {
                return;
            }

            room.SendPacket(new UserTypingComposer(roomUserByUserId.VirtualId, false));
        }
    }
}
