using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderation;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class ModerationMuteEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetUser().HasPermission("perm_no_kick"))
            {
                return;
            }

            ModerationManager.KickUser(Session, Packet.PopInt(), Packet.PopString(), false);
        }
    }
}
