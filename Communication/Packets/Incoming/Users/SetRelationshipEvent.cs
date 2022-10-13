namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Database.Daos.Messenger;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users.Messenger;

internal class SetRelationshipEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.GetUser() == null || session.GetUser().Messenger == null)
        {
            return;
        }

        var user = packet.PopInt();
        var type = packet.PopInt();

        if (type is < 0 or > 3)
        {
            return;
        }

        if (!session.GetUser().Messenger.FriendshipExists(user))
        {
            return;
        }

        if (type == 0)
        {
            if (session.GetUser().Messenger.Relation.ContainsKey(user))
            {
                _ = session.GetUser().Messenger.Relation.Remove(user);
            }
        }
        else
        {
            if (session.GetUser().Messenger.Relation.ContainsKey(user))
            {
                session.GetUser().Messenger.Relation[user].Type = type;
            }
            else
            {
                session.GetUser().Messenger.Relation.Add(user, new Relationship(user, type));
            }
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            MessengerFriendshipDao.UpdateRelation(dbClient, type, session.GetUser().Id, user);
        }

        session.GetUser().
        Messenger.RelationChanged(user, type);
        session.GetUser().Messenger.UpdateFriend(user, true);
    }
}
