using Butterfly.Communication.Packets.Outgoing.Notifications;
using Butterfly.Communication.Packets.Outgoing.Rooms.Session;

using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class FindRandomFriendingRoomEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            string type = Packet.PopString();

            if (type == "predefined_noob_lobby")
            {
                Session.SendPacket(new InClientLinkComposer("nux/lobbyoffer/hide"));
            }

            Room Instance = ButterflyEnvironment.GetGame().GetRoomManager().TryGetRandomLoadedRoom();

            if (Instance != null)
            {
                Session.SendPacket(new RoomForwardComposer(Instance.Id));
            }
        }
    }
}