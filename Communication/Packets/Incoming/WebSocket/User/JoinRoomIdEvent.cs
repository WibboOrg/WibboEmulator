using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using Butterfly.Game.WebClients;

namespace Butterfly.Communication.Packets.Incoming.WebSocket
{
    internal class JoinRoomIdEvent : IPacketWebEvent
    {
        public double Delay => 0;

        public void Parse(WebClient Session, ClientPacket Packet)
        {
            Client Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null || Client.GetUser() == null)
            {
                return;
            }

            int RoomId = Packet.PopInt();

            Room Room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(RoomId);
            if (Room == null)
            {
                return;
            }

            Client.SendPacket(new GetGuestRoomResultComposer(Client, Room.RoomData, false, true));
        }
    }
}
