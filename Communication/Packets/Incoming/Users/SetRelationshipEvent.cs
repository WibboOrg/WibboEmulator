namespace WibboEmulator.Communication.Packets.Incoming.Users;

using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Messenger;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users.Messenger;

internal sealed class SetRelationshipEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User == null || Session.User.Messenger == null)
        {
            return;
        }

        var user = packet.PopInt();
        var type = packet.PopInt();

        if (type is < 0 or > 3)
        {
            return;
        }

        if (!Session.User.Messenger.FriendshipExists(user))
        {
            return;
        }

        if (type == 0)
        {
            if (Session.User.Messenger.Relation.ContainsKey(user))
            {
                _ = Session.User.Messenger.Relation.Remove(user);
            }
        }
        else
        {
            if (Session.User.Messenger.Relation.TryGetValue(user, out var value))
            {
                value.Type = type;
            }
            else
            {
                Session.User.Messenger.Relation.Add(user, new Relationship(user, type));
            }
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            MessengerFriendshipDao.UpdateRelation(dbClient, type, Session.User.Id, user);
        }

        Session.User.Messenger.RelationChanged(user, type);
        Session.User.Messenger.UpdateFriend(user, true);
    }
}
