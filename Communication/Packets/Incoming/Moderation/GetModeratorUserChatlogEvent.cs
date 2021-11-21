using Butterfly.Game.Clients;
using Butterfly.Game.Moderation;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetModeratorUserChatlogEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_chatlogs"))
            {
                return;
            }

            Session.SendPacket(ModerationManager.SerializeUserChatlog(Packet.PopInt(), Session.GetHabbo().CurrentRoomId));
        }
    }
}
