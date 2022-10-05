namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users.Relationships;

internal class SetRelationshipEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session.GetUser() == null || session.GetUser().GetMessenger() == null)
        {
            return;
        }

        var User = Packet.PopInt();
        var Type = Packet.PopInt();

        if (Type is < 0 or > 3)
        {
            return;
        }

        if (!session.GetUser().GetMessenger().FriendshipExists(User))
        {
            return;
        }

        if (Type == 0)
        {
            if (session.GetUser().GetMessenger().Relation.ContainsKey(User))
            {
                session.GetUser().GetMessenger().Relation.Remove(User);
            }
        }
        else
        {
            if (session.GetUser().GetMessenger().Relation.ContainsKey(User))
            {
                session.GetUser().GetMessenger().Relation[User].Type = Type;
            }
            else
            {
                session.GetUser().GetMessenger().Relation.Add(User, new Relationship(User, Type));
            }
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            MessengerFriendshipDao.UpdateRelation(dbClient, Type, session.GetUser().Id, User);
        }

        session.GetUser().GetMessenger().RelationChanged(User, Type);
        session.GetUser().GetMessenger().UpdateFriend(User, true);
    }
}
