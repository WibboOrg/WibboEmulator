namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Connection;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Games.GameClients;

internal sealed class GoToFlatEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!Session.User.InRoom)
        {
            return;
        }

        var room = Session.User.Room;

        if (room == null)
        {
            return;
        }

        if (!Session.User.EnterRoom(room))
        {
            Session.SendPacket(new CloseConnectionComposer());
        }
    }
}
