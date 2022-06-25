using WibboEmulator.Game.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class PickTicketEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetUser().HasFuse("fuse_mod"))
            {
                return;
            }

            Packet.PopInt();
            WibboEnvironment.GetGame().GetModerationManager().PickTicket(Session, Packet.PopInt());
        }
    }
}
