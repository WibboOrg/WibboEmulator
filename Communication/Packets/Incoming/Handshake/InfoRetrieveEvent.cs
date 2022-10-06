namespace WibboEmulator.Communication.Packets.Incoming.Handshake;
using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Games.GameClients;

internal class InfoRetrieveEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        session.SendPacket(new UserObjectComposer(session.GetUser()));
        session.SendPacket(new UserPerksComposer(session.GetUser()));
    }
}
