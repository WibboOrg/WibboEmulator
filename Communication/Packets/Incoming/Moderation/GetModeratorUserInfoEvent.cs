namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users;

internal sealed class GetModeratorUserInfoEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!Session.User.HasPermission("mod"))
        {
            return;
        }

        var userId = packet.PopInt();

        var user = UserManager.GetUserById(userId);

        if (user == null)
        {
            Session.SendNotification(LanguageManager.TryGetValue("user.loadusererror", Session.Language));
            return;
        }

        Session.SendPacket(new ModeratorUserInfoComposer(user));
    }
}
