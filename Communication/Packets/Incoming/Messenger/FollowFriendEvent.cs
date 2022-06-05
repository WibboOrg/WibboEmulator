using Wibbo.Communication.Packets.Outgoing.Rooms.Session;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class FollowFriendEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int userId = Packet.PopInt();
            Client clientByUserId = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(userId);
            if (clientByUserId == null || clientByUserId.GetUser() == null || !clientByUserId.GetUser().InRoom || (clientByUserId.GetUser().HideInRoom && !Session.GetUser().HasFuse("fuse_mod")))
            {
                return;
            }

            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(clientByUserId.GetUser().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            Session.SendPacket(new RoomForwardComposer(clientByUserId.GetUser().CurrentRoomId));
        }
    }
}