using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Rooms.Chat;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class CancelTypingEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabbo == null)
            {
                return;
            }

            room.SendPacket(new UserTypingComposer(roomUserByHabbo.VirtualId, 0));
        }
    }
}
