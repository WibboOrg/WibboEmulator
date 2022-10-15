namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;

internal class OpenHelpToolEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.User == null || session.User.HasPermission("perm_helptool"))
        {
            return;
        }

        session.SendPacket(new OpenHelpToolComposer(0));
    }
}
