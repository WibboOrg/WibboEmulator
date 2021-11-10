using Butterfly.Game.GameClients;
using Butterfly.Game.Support;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetModeratorUserChatlogEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_chatlogs"))
            {
                return;
            }

            Session.SendPacket(ModerationManager.SerializeUserChatlog(Packet.PopInt(), Session.GetHabbo().CurrentRoomId));
        }
    }
}
