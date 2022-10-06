namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;

internal class CheckValidNameEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.GetUser() == null || session == null)
        {
            return;
        }

        var Name = packet.PopString();

        session.SendPacket(new NameChangeUpdateComposer(Name));
    }
}