namespace WibboEmulator.Communication.Packets.Incoming.Misc;
using WibboEmulator.Communication.Packets.Outgoing.Misc;
using WibboEmulator.Games.GameClients;

internal sealed class LatencyTestEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet) => session.SendPacket(new LatencyResponseComposer(packet.PopInt()));
}