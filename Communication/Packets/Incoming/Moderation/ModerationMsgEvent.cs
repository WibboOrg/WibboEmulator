namespace WibboEmulator.Communication.Packets.Incoming.Moderation;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderations;

internal sealed class ModerationMsgEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!Session.User.HasPermission("alert"))
        {
            return;
        }

        var userId = packet.PopInt();
        var message = packet.PopString();

        var clientTarget = GameClientManager.GetClientByUserID(userId);
        if (clientTarget == null)
        {
            return;
        }

        if (clientTarget.User.Id == Session.User.Id)
        {
            return;
        }

        if (clientTarget.User.Rank >= Session.User.Rank)
        {
            Session.SendNotification(LanguageManager.TryGetValue("moderation.caution.missingrank", Session.Language));
        }

        if (Session.User.CheckChatMessage(message, "<MT>"))
        {
            return;
        }

        ModerationManager.LogStaffEntry(Session.User.Id, Session.User.Username, 0, string.Empty, "ModTool", string.Format("Modtool alert ( {1} ): {0}", message, clientTarget.User.Username));

        clientTarget.SendNotification(message);
    }
}
