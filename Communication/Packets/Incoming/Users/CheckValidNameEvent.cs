namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;

internal sealed class CheckValidNameEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User == null || Session == null)
        {
            return;
        }

        var name = packet.PopString(16);

        Session.SendPacket(new NameChangeUpdateComposer(name));
    }
}
