using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class FollowFriendEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int userId = Packet.PopInt();
            GameClient clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId);
            if (clientByUserId == null || clientByUserId.GetUser() == null || !clientByUserId.GetUser().InRoom || (clientByUserId.GetUser().HideInRoom && !Session.GetUser().HasPermission("perm_mod")))
            {
                return;
            }

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(clientByUserId.GetUser().CurrentRoomId, out Room room))
                return;

            Session.SendPacket(new RoomForwardComposer(room.Id));
        }
    }
}