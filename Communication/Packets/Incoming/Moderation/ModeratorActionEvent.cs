namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderation;

internal class ModeratorActionEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.GetUser().HasPermission("perm_alert"))
        {
            return;
        }

        var alertMode = packet.PopInt();
        var alertMessage = packet.PopString();
        var isCaution = alertMode != 3;

        if (session.Antipub(alertMessage, "<MT>"))
        {
            return;
        }

        ModerationManager.LogStaffEntry(session.GetUser().Id, session.GetUser().Username, 0, string.Empty, alertMessage.Split(' ')[0].Replace(":", ""), string.Format("Modtool Roomalert: {0}", alertMessage));

        session.GetUser().CurrentRoom.SendPacket(new BroadcastMessageAlertComposer(alertMessage));
    }
}
