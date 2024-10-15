namespace WibboEmulator.Communication.Packets.Incoming.Settings;
using WibboEmulator.Games.GameClients;

internal sealed class UserSettingsOldChatEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {

    }
}
