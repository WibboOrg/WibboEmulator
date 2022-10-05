namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.GameClients;

internal class ModeratorActionEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (!session.GetUser().HasPermission("perm_alert"))
        {
            return;
        }

        var AlertMode = Packet.PopInt();
        var AlertMessage = Packet.PopString();
        var IsCaution = AlertMode != 3;

        if (session.Antipub(AlertMessage, "<MT>"))
        {
            return;
        }

        WibboEnvironment.GetGame().GetModerationManager().LogStaffEntry(session.GetUser().Id, session.GetUser().Username, 0, string.Empty, AlertMessage.Split(' ')[0].Replace(":", ""), string.Format("Modtool Roomalert: {0}", AlertMessage));

        session.GetUser().CurrentRoom.SendPacket(new BroadcastMessageAlertComposer(AlertMessage));
    }
}
