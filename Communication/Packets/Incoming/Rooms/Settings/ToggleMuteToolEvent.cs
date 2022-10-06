namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Settings;
using WibboEmulator.Games.GameClients;

internal class ToggleMuteToolEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {

    }
}
