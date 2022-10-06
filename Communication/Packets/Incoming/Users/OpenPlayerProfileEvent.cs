namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Database.Daos.Messenger;
using WibboEmulator.Games.GameClients;

internal class OpenPlayerProfileEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var userId = packet.PopInt();
        var IsMe = packet.PopBoolean();

        var targetData = WibboEnvironment.GetUserById(userId);
        if (targetData == null)
        {
            return;
        }

        var Groups = WibboEnvironment.GetGame().GetGroupManager().GetGroupsForUser(targetData.MyGroups);

        var friendCount = 0;

        if (targetData.GetMessenger() != null)
        {
            friendCount = targetData.GetMessenger().Count;
        }
        else
        {
            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            friendCount = MessengerFriendshipDao.GetCount(dbClient, userId);
        }

        session.SendPacket(new ProfileInformationComposer(targetData, session, Groups, friendCount));
    }
}