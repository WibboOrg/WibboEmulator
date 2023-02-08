namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Database.Daos.Messenger;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users.Messenger;

internal class SetRelationshipEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.User == null || session.User.Messenger == null)
        {
            return;
        }

        var user = packet.PopInt();
        var type = packet.PopInt();

        if (type is < 0 or > 3)
        {
            return;
        }

        if (!session.User.Messenger.FriendshipExists(user))
        {
            return;
        }

        if (type == 0)
        {
            if (session.User.Messenger.Relation.ContainsKey(user))
            {
                _ = session.User.Messenger.Relation.Remove(user);
            }
        }
        else
        {
            if (session.User.Messenger.Relation.TryGetValue(user, out var value))
            {
                value.Type = type;
            }
            else
            {
                session.User.Messenger.Relation.Add(user, new Relationship(user, type));
            }
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            MessengerFriendshipDao.UpdateRelation(dbClient, type, session.User.Id, user);
        }

        session.User.Messenger.RelationChanged(user, type);
        session.User.Messenger.UpdateFriend(user, true);
    }
}
