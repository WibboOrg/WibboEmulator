using Butterfly.Database.Interfaces;using Butterfly.HabboHotel.GameClients;using Butterfly.HabboHotel.Users.Messenger;namespace Butterfly.Communication.Packets.Incoming.Structure{    internal class SetRelationshipEvent : IPacketEvent    {        public void Parse(GameClient Session, ClientPacket Packet)        {            if (Session.GetHabbo() == null || Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            int User = Packet.PopInt();            int Type = Packet.PopInt();            if (Type < 0 || Type > 3)
            {
                return;
            }

            if (!Session.GetHabbo().GetMessenger().FriendshipExists(User))
            {
                return;
            }

            if (Type == 0)            {                if (Session.GetHabbo().GetMessenger().relation.ContainsKey(User))                {                    Session.GetHabbo().GetMessenger().relation.Remove(User);                }            }            else            {                if (Session.GetHabbo().GetMessenger().relation.ContainsKey(User))                {                    Session.GetHabbo().GetMessenger().relation[User].Type = Type;                }                else                {                    Session.GetHabbo().GetMessenger().relation.Add(User, new Relationship(User, Type));                }            }            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())            {                dbClient.SetQuery("UPDATE messenger_friendships SET relation = '" + Type + "' WHERE user_one_id=@id AND user_two_id=@target LIMIT 1");                dbClient.AddParameter("id", Session.GetHabbo().Id);                dbClient.AddParameter("target", User);                dbClient.RunQuery();            }            Session.GetHabbo().GetMessenger().RelationChanged(User, Type);            Session.GetHabbo().GetMessenger().UpdateFriend(User, true);        }    }}