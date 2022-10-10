namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

internal class GetModeratorUserInfoEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.GetUser().HasPermission("perm_mod"))
        {
            return;
        }

        var userId = packet.PopInt();
        if (WibboEnvironment.GetGame().GetGameClientManager().GetNameById(userId) != "")
        {
            _ = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId);
            DataRow user = null;
            DataRow info = null;

            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                user = UserDao.GetOneInfo(dbClient, userId);
            }

            if (user == null)
            {
                return;
            }

            session.SendPacket(new ModeratorUserInfoComposer(user, info));
        }
        else
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("user.loadusererror", session.Langue));
        }
    }
}
