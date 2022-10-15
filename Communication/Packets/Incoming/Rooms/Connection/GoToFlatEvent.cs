namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Connection;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Games.GameClients;

internal class GoToFlatEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.InRoom)
        {
            return;
        }

        if (!session.User.EnterRoom(session.User.CurrentRoom))
        {
            session.SendPacket(new CloseConnectionComposer());
        }
    }
}
