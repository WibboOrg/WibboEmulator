using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Support;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ModerationMuteEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_no_kick"))
            {
                return;
            }

            ModerationManager.KickUser(Session, Packet.PopInt(), Packet.PopString(), false);
        }
    }
}
