namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderation;

internal class ModerationMsgEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.GetUser().HasPermission("perm_alert"))
        {
            return;
        }

        var userId = packet.PopInt();
        var message = packet.PopString();

        var clientTarget = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId);
        if (clientTarget == null)
        {
            return;
        }

        if (clientTarget.GetUser().Id == session.GetUser().Id)
        {
            return;
        }

        if (clientTarget.GetUser().Rank >= session.GetUser().Rank)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("moderation.caution.missingrank", session.Langue));
        }

        if (session.Antipub(message, "<MT>"))
        {
            return;
        }

        ModerationManager.LogStaffEntry(session.GetUser().Id, session.GetUser().Username, 0, string.Empty, "ModTool", string.Format("Modtool alert ( {1} ): {0}", message, clientTarget.GetUser().Username));

        clientTarget.SendNotification(message);
    }
}
