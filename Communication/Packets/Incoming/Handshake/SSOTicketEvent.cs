using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class SSOTicketEvent : IPacketEvent
    {
        public double Delay => 5000;

        public void Parse(GameClient Session, ClientPacket Packet)
        {            if (Session == null || Session.GetUser() != null)
            {
                return;
            }

            string SSOTicket = Packet.PopString();            int Timer = Packet.PopInt();

            Session.TryAuthenticate(SSOTicket);
        }
    }
}
