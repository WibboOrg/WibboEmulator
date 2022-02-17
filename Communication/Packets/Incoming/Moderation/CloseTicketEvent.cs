using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class CloseTicketEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            int Result = Packet.PopInt();
            Packet.PopInt();
            int TicketId = Packet.PopInt();

            ButterflyEnvironment.GetGame().GetModerationManager().CloseTicket(Session, Packet.PopInt(), Result);
        }
    }
}
