namespace WibboEmulator.Communication.Packets.Incoming.Handshake;
using WibboEmulator.Games.GameClients;

internal sealed class GetClientVersionEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient Session, ClientPacket packet)
    {
    }
}
