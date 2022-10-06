namespace WibboEmulator.Communication.Packets.Incoming.Handshake;
using WibboEmulator.Games.GameClients;

internal class SSOTicketEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient session, ClientPacket Packet)
    {        if (session == null || session.GetUser() != null)
        {
            return;
        }

        var SSOTicket = Packet.PopString();        var Timer = Packet.PopInt();

        session.TryAuthenticate(SSOTicket);
    }
}
