using Butterfly.Communication.Packets.Outgoing.Notifications;
using Butterfly.Communication.Packets.Outgoing.Rooms.Session;

using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class FindRandomFriendingRoomEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string type = Packet.PopString();

            if (type == "predefined_noob_lobby")
            {
                Session.SendPacket(new NuxAlertComposer("nux/lobbyoffer/hide"));
            }

            Room Instance = ButterflyEnvironment.GetGame().GetRoomManager().TryGetRandomLoadedRoom();

            if (Instance != null)
            {
                Session.SendPacket(new RoomForwardComposer(Instance.Id));
            }
        }
    }
}