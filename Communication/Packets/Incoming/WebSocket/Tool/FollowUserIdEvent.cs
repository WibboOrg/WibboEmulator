using Butterfly.Communication.Packets.Outgoing.Rooms.Session;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using Butterfly.Game.WebClients;

namespace Butterfly.Communication.Packets.Incoming.WebSocket
{
    internal class FollowUserIdEvent : IPacketWebEvent
    {
        public double Delay => 0;

        public void Parse(WebClient Session, ClientPacket Packet)
        {
            Client client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (client == null || client.GetUser() == null)
            {
                return;
            }

            int userId = Packet.PopInt();
            Client clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(userId);
            if (clientByUserId == null || clientByUserId.GetUser() == null || !clientByUserId.GetUser().InRoom || (clientByUserId.GetUser().HideInRoom && !client.GetUser().HasFuse("fuse_mod")))
            {
                return;
            }

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(clientByUserId.GetUser().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            client.SendPacket(new RoomForwardComposer(clientByUserId.GetUser().CurrentRoomId));
        }
    }
}
