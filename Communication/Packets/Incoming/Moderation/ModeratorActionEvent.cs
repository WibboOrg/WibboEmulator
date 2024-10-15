namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderations;

internal sealed class ModeratorActionEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!Session.User.HasPermission("alert"))
        {
            return;
        }

        var alertMode = packet.PopInt();
        var alertMessage = packet.PopString();

        _ = alertMode != 3;

        if (Session.User.CheckChatMessage(alertMessage, "<MT>"))
        {
            return;
        }

        ModerationManager.LogStaffEntry(Session.User.Id, Session.User.Username, 0, string.Empty, alertMessage.Split(' ')[0].Replace(":", ""), string.Format("Modtool Roomalert: {0}", alertMessage));

        Session.User.Room.SendPacket(new BroadcastMessageAlertComposer(alertMessage));
    }
}
