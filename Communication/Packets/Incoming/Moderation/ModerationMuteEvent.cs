using Wibbo.Game.Clients;
using Wibbo.Game.Moderation;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class ModerationMuteEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetUser().HasFuse("fuse_no_kick"))
            {
                return;
            }

            ModerationManager.KickUser(Session, Packet.PopInt(), Packet.PopString(), false);
        }
    }
}
