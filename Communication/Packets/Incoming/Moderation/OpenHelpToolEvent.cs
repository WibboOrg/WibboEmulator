namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;

internal sealed class OpenHelpToolEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User == null || Session.User.HasPermission("helptool"))
        {
            return;
        }

        Session.SendPacket(new OpenHelpToolComposer(0));
    }
}
