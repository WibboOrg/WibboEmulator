namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Games.GameClients;

internal class ModerationMuteEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.GetUser().HasPermission("perm_no_kick"))
        {
            return;
        }

        WibboEnvironment.GetGame().GetModerationManager().KickUser(session, packet.PopInt(), packet.PopString(), false);
    }
}
