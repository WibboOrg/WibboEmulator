namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Connection;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.session;
using WibboEmulator.Games.GameClients;

internal class GoToFlatEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.GetUser().InRoom)
        {
            return;
        }

        if (!session.GetUser().EnterRoom(session.GetUser().CurrentRoom))
        {
            session.SendPacket(new CloseConnectionComposer());
        }
    }
}
