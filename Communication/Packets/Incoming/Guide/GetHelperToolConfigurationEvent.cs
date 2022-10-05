namespace WibboEmulator.Communication.Packets.Incoming.Guide;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;

internal class GetHelperToolConfigurationEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (!session.GetUser().HasPermission("perm_helptool"))
        {
            return;
        }

        var guideManager = WibboEnvironment.GetGame().GetHelpManager();
        var onDuty = Packet.PopBoolean();
        Packet.PopBoolean();
        Packet.PopBoolean();
        Packet.PopBoolean();

        if (onDuty && !session.GetUser().OnDuty)
        {
            guideManager.AddGuide(session.GetUser().Id);
            session.GetUser().OnDuty = true;
        }
        else
        {
            guideManager.RemoveGuide(session.GetUser().Id);
            session.GetUser().OnDuty = false;
        }

        session.SendPacket(new HelperToolComposer(session.GetUser().OnDuty, guideManager.GuidesCount));
    }
}