using Butterfly.Communication.Packets.Outgoing.Rooms.Session;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class FindNewFriendsEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.SendPacket(new RoomForwardComposer(447654));
        }
    }
}
