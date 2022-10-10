namespace WibboEmulator.Communication.Packets.Incoming.Handshake;
using WibboEmulator.Games.GameClients;

internal class GetClientVersionEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        _ = packet.PopString();

        _ = packet.PopString();

        _ = packet.PopInt();

        _ = packet.PopInt();

    }
}
