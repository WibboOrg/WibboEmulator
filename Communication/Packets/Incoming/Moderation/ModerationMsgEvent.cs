using Butterfly.Game.GameClients;
using Butterfly.Game.Moderation;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ModerationMsgEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_alert"))
            {
                return;
            }

            ModerationManager.AlertUser(Session, Packet.PopInt(), Packet.PopString(), true);
        }
    }
}
