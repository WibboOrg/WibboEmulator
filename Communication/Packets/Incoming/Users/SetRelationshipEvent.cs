namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Database.Daos.Messenger;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users.Relationships;

internal class SetRelationshipEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.GetUser() == null || session.GetUser().GetMessenger() == null)
        {
            return;
        }

        var user = packet.PopInt();
        var type = packet.PopInt();

        if (type is < 0 or > 3)
        {
            return;
        }

        if (!session.GetUser().GetMessenger().FriendshipExists(user))
        {
            return;
        }

        if (type == 0)
        {
            if (session.GetUser().GetMessenger().Relation.ContainsKey(user))
            {
                _ = session.GetUser().GetMessenger().Relation.Remove(user);
            }
        }
        else
        {
            if (session.GetUser().GetMessenger().Relation.ContainsKey(user))
            {
                session.GetUser().GetMessenger().Relation[user].Type = type;
            }
            else
            {
                session.GetUser().GetMessenger().Relation.Add(user, new Relationship(user, type));
            }
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            MessengerFriendshipDao.UpdateRelation(dbClient, type, session.GetUser().Id, user);
        }

        session.GetUser().GetMessenger().RelationChanged(user, type);
        session.GetUser().GetMessenger().UpdateFriend(user, true);
    }
}
