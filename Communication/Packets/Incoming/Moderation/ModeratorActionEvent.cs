namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderations;

internal sealed class ModeratorActionEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.HasPermission("alert"))
        {
            return;
        }

        var alertMode = packet.PopInt();
        var alertMessage = packet.PopString();

        _ = alertMode != 3;

        if (session.User.Antipub(alertMessage, "<MT>"))
        {
            return;
        }

        ModerationManager.LogStaffEntry(session.User.Id, session.User.Username, 0, string.Empty, alertMessage.Split(' ')[0].Replace(":", ""), string.Format("Modtool Roomalert: {0}", alertMessage));

        session.User.CurrentRoom.SendPacket(new BroadcastMessageAlertComposer(alertMessage));
    }
}
