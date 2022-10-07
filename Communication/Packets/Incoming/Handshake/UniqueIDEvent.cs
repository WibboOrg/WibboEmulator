namespace WibboEmulator.Communication.Packets.Incoming.Handshake;
using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Core;
using WibboEmulator.Games.GameClients;

internal class UniqueIDEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var cookieId = packet.PopString();
        var mcId = packet.PopString();
        var junk = packet.PopString();

        var head = string.IsNullOrWhiteSpace(cookieId) || cookieId.Length != 13 ? IDGenerator.Next : cookieId;

        session.MachineId = head + mcId + junk;

        session.SendPacket(new SetUniqueIdComposer(head));
    }
}
