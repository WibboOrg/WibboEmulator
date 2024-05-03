namespace WibboEmulator.Communication.Packets.Incoming.Guide;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Helps;

internal sealed class GetHelperToolConfigurationEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.HasPermission("helptool"))
        {
            return;
        }

        var onDuty = packet.PopBoolean();
        _ = packet.PopBoolean();
        _ = packet.PopBoolean();
        _ = packet.PopBoolean();

        if (onDuty && !session.User.OnDuty)
        {
            HelpManager.TryAddGuide(session.User.Id);
            session.User.OnDuty = true;
        }
        else
        {
            HelpManager.TryRemoveGuide(session.User.Id);
            session.User.OnDuty = false;
        }

        session.SendPacket(new HelperToolComposer(session.User.OnDuty, HelpManager.Count));
    }
}
