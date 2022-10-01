using WibboEmulator.Games.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class PickTicketEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetUser().HasPermission("perm_mod"))
            {
                return;
            }

            Packet.PopInt();
            WibboEnvironment.GetGame().GetModerationManager().PickTicket(Session, Packet.PopInt());
        }
    }
}
