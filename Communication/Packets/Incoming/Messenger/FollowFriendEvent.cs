using Butterfly.Communication.Packets.Outgoing.Rooms.Session;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class FollowFriendEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            Client clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Packet.PopInt());
            if (clientByUserId == null || clientByUserId.GetHabbo() == null || !clientByUserId.GetHabbo().InRoom || (clientByUserId.GetHabbo().HideInRoom && !Session.GetHabbo().HasFuse("fuse_mod")))
            {
                return;
            }

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(clientByUserId.GetHabbo().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            Session.SendPacket(new RoomForwardComposer(clientByUserId.GetHabbo().CurrentRoomId));
        }
    }
}