namespace WibboEmulator.Communication.Packets.Incoming.Guide;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Helps;

internal sealed class GetHelperToolConfigurationEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!Session.User.HasPermission("helptool"))
        {
            return;
        }

        var onDuty = packet.PopBoolean();
        _ = packet.PopBoolean();
        _ = packet.PopBoolean();
        _ = packet.PopBoolean();

        if (onDuty && !Session.User.OnDuty)
        {
            HelpManager.TryAddGuide(Session.User.Id);
            Session.User.OnDuty = true;
        }
        else
        {
            HelpManager.TryRemoveGuide(Session.User.Id);
            Session.User.OnDuty = false;
        }

        Session.SendPacket(new HelperToolComposer(Session.User.OnDuty, HelpManager.Count));
    }
}
