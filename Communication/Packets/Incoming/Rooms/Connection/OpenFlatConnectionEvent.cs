namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Connection;
using WibboEmulator.Games.GameClients;

internal class OpenFlatConnectionEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null)
        {
            return;
        }

        var roomId = packet.PopInt();
        var password = packet.PopString();

        session.User.PrepareRoom(roomId, password);
    }
}
