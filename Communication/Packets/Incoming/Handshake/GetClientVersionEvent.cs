namespace WibboEmulator.Communication.Packets.Incoming.Handshake;
using WibboEmulator.Games.GameClients;

internal class GetClientVersionEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var release = packet.PopString();
        var type = packet.PopString();
        var platform = packet.PopInt();
        var category = packet.PopInt();

    }
}
