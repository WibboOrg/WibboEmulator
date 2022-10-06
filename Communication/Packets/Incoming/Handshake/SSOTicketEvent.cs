namespace WibboEmulator.Communication.Packets.Incoming.Handshake;
using WibboEmulator.Games.GameClients;

internal class SSOTicketEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient session, ClientPacket packet)
    {        if (session == null || session.GetUser() != null)
        {
            return;
        }

        var SSOTicket = packet.PopString();        var Timer = packet.PopInt();

        session.TryAuthenticate(SSOTicket);
    }
}
