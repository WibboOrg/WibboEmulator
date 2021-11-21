using Butterfly.Game.Clients;
using Butterfly.Game.Moderation;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ModerateRoomEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
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
