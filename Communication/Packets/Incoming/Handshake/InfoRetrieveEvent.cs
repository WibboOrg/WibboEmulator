namespace WibboEmulator.Communication.Packets.Incoming.Handshake;
using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Games.GameClients;

internal sealed class InfoRetrieveEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        Session.SendPacket(new UserObjectComposer(Session.User));
        Session.SendPacket(new UserPerksComposer(Session.User));
    }
}
