namespace WibboEmulator.Communication.Packets.Incoming.Guide;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;

internal sealed class GetHelperToolConfigurationEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.HasPermission("helptool"))
        {
            return;
        }

        var guideManager = WibboEnvironment.GetGame().GetHelpManager();
        var onDuty = packet.PopBoolean();
        _ = packet.PopBoolean();
        _ = packet.PopBoolean();
        _ = packet.PopBoolean();

        if (onDuty && !session.User.OnDuty)
        {
            guideManager.AddGuide(session.User.Id);
            session.User.OnDuty = true;
        }
        else
        {
            guideManager.RemoveGuide(session.User.Id);
            session.User.OnDuty = false;
        }

        session.SendPacket(new HelperToolComposer(session.User.OnDuty, guideManager.GuidesCount));
    }
}