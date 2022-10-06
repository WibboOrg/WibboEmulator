namespace WibboEmulator.Communication.Packets.Incoming.Handshake;
using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Core;
using WibboEmulator.Games.GameClients;

internal class UniqueIDEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var CookieId = packet.PopString();
        var McId = packet.PopString();
        var Junk = packet.PopString();

        var Head = string.IsNullOrWhiteSpace(CookieId) || CookieId.Length != 13 ? IDGenerator.Next : CookieId;

        session.MachineId = Head + McId + Junk;

        session.SendPacket(new SetUniqueIdComposer(Head));
    }
}
