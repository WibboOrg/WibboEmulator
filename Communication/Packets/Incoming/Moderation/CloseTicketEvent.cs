using Butterfly.Game.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class CloseTicketEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            int Result = Packet.PopInt();
            Packet.PopInt();

            ButterflyEnvironment.GetGame().GetModerationManager().CloseTicket(Session, Packet.PopInt(), Result);
        }
    }
}
