namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Games.GameClients;

internal class OpenFlatConnectionEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session == null || session.GetUser() == null)
        {
            return;
        }

        var RoomId = Packet.PopInt();
        var Password = Packet.PopString();

        session.GetUser().PrepareRoom(RoomId, Password);
    }
}