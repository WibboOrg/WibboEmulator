namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;

internal class CheckValidNameEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session.GetUser() == null || session == null)
        {
            return;
        }

        var Name = Packet.PopString();

        session.SendPacket(new NameChangeUpdateComposer(Name));
    }
}