using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Users.Messenger;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class SetRelationshipEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetUser() == null || Session.GetUser().GetMessenger() == null)
            {
                return;
            }

            int User = Packet.PopInt();
            int Type = Packet.PopInt();

            if (Type < 0 || Type > 3)
            {
                return;
            }

            if (!Session.GetUser().GetMessenger().FriendshipExists(User))
            {
                return;
            }

            if (Type == 0)
            {
                if (Session.GetUser().GetMessenger().Relation.ContainsKey(User))
                {
                    Session.GetUser().GetMessenger().Relation.Remove(User);
                }
            }
            else
            {
                if (Session.GetUser().GetMessenger().Relation.ContainsKey(User))
                {
                    Session.GetUser().GetMessenger().Relation[User].Type = Type;
                }
                else
                {
                    Session.GetUser().GetMessenger().Relation.Add(User, new Relationship(User, Type));
                }
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                MessengerFriendshipDao.UpdateRelation(dbClient, Type, Session.GetUser().Id, User);
            }

            Session.GetUser().GetMessenger().RelationChanged(User, Type);
            Session.GetUser().GetMessenger().UpdateFriend(User, true);
        }
    }
}
