using WibboEmulator.Game.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class CloseTicketEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null || !Session.GetUser().HasFuse("fuse_mod"))
            {
                return;
            }

            int Result = Packet.PopInt();
            Packet.PopInt();
            int TicketId = Packet.PopInt();

            WibboEnvironment.GetGame().GetModerationManager().CloseTicket(Session, Packet.PopInt(), Result);
        }
    }
}
