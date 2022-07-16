using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Moderation;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class ModerateRoomEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetUser().HasPermission("perm_mod"))
            {
                return;
            }

            int RoomId = Packet.PopInt();
            bool LockRoom = Packet.PopInt() == 1;
            bool InappropriateRoom = Packet.PopInt() == 1;
            bool KickUsers = Packet.PopInt() == 1;

            ModerationManager.PerformRoomAction(Session, RoomId, KickUsers, LockRoom, InappropriateRoom);
        }
    }
}
