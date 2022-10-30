namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

internal class GetModeratorUserInfoEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.HasPermission("mod"))
        {
            return;
        }

        var userId = packet.PopInt();
        if (WibboEnvironment.GetGame().GetGameClientManager().GetNameById(userId) != "")
        {
            _ = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId);

            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            var user = UserDao.GetOneInfo(dbClient, userId);

            if (user == null)
            {
                return;
            }

            session.SendPacket(new ModeratorUserInfoComposer(user));
        }
        else
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("user.loadusererror", session.Langue));
        }
    }
}
