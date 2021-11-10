using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Game.GameClients;
using Butterfly.Game.Rooms;
using Butterfly.Game.WebClients;

namespace Butterfly.Communication.Packets.Incoming.WebSocket
{
    internal class JoinRoomIdEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            GameClient Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null || Client.GetHabbo() == null)
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
