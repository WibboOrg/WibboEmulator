using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class FollowFriendEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int userId = Packet.PopInt();
            Client clientByUserId = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(userId);
            if (clientByUserId == null || clientByUserId.GetUser() == null || !clientByUserId.GetUser().InRoom || (clientByUserId.GetUser().HideInRoom && !Session.GetUser().HasPermission("perm_mod")))
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