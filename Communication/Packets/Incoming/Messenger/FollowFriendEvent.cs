using Butterfly.Communication.Packets.Outgoing.Rooms.Session;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class FollowFriendEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int userId = Packet.PopInt();
            Client clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(userId);
            if (clientByUserId == null || clientByUserId.GetUser() == null || !clientByUserId.GetUser().InRoom || (clientByUserId.GetUser().HideInRoom && !Session.GetUser().HasFuse("fuse_mod")))
            {
                return;
            }

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(clientByUserId.GetUser().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            Session.SendPacket(new RoomForwardComposer(clientByUserId.GetUser().CurrentRoomId));
        }
    }
}