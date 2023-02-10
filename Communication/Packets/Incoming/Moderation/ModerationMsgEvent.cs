namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderations;

internal sealed class ModerationMsgEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.HasPermission("alert"))
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

        if (clientTarget.User.Id == session.User.Id)
        {
            return;
        }

        if (clientTarget.User.Rank >= session.User.Rank)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("moderation.caution.missingrank", session.Langue));
        }

        if (session.Antipub(message, "<MT>"))
        {
            return;
        }

        ModerationManager.LogStaffEntry(session.User.Id, session.User.Username, 0, string.Empty, "ModTool", string.Format("Modtool alert ( {1} ): {0}", message, clientTarget.User.Username));

        clientTarget.SendNotification(message);
    }
}
