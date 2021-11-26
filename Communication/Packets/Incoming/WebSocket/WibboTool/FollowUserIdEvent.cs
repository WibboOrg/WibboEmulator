using Butterfly.Communication.Packets.Outgoing.Rooms.Session;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using Butterfly.Game.WebClients;

namespace Butterfly.Communication.Packets.Incoming.WebSocket
{
    internal class FollowUserIdEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            Client client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (client == null || client.GetHabbo() == null)
            {
                return;
            }

            int userId = Packet.PopInt();
            Client clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(userId);
            if (clientByUserId == null || clientByUserId.GetHabbo() == null || !clientByUserId.GetHabbo().InRoom || (clientByUserId.GetHabbo().HideInRoom && !client.GetHabbo().HasFuse("fuse_mod")))
            {
                return;
            }

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(clientByUserId.GetHabbo().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            client.SendPacket(new RoomForwardComposer(clientByUserId.GetHabbo().CurrentRoomId));
        }
    }
}
