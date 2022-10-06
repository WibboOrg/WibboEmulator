namespace WibboEmulator.Communication.Packets.Incoming.Handshake;
using WibboEmulator.Games.GameClients;

internal class PingEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {

    }
}
