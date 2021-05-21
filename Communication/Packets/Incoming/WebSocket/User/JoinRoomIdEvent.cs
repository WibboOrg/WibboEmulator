using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.WebClients;

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
