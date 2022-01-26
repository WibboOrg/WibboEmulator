using Butterfly.Game.Users;
using Butterfly.Game.Users.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Butterfly.Communication.Packets.Outgoing.Users
{
    internal class GetRelationshipsComposer : ServerPacket
    {
        public GetRelationshipsComposer(int Id, int Nbrela, Dictionary<int, Relationship> Loves, Dictionary<int, Relationship> Likes, Dictionary<int, Relationship> Hates)
            : base(ServerPacketHeader.MESSENGER_RELATIONSHIPS)
        {
            this.WriteInteger(Id);
            this.WriteInteger(Nbrela); // Count //Habbo.Relationships.Count

            if (Loves.Count > 0)
            {
                //Loves
                this.WriteInteger(1); //type
                this.WriteInteger(Loves.Count); //Total personne love

                Random randlove = new Random();
                int useridlove = Loves.ElementAt(randlove.Next(0, Loves.Count)).Value.UserId;//Loves[randlove.Next(Loves.Count)].UserId;
                User HHablove = ButterflyEnvironment.GetHabboById(Convert.ToInt32(useridlove));
                this.WriteInteger(useridlove); // UserId
                this.WriteString((HHablove == null) ? "" : HHablove.Username);
                this.WriteString((HHablove == null) ? "" : HHablove.Look); // look
            }
            if (Likes.Count > 0)
            {
                //Likes
                this.WriteInteger(2); //type
                this.WriteInteger(Likes.Count); //Total personne Like

                Random randLikes = new Random();
                int useridLikes = Likes.ElementAt(randLikes.Next(0, Likes.Count)).Value.UserId;//Likes[randLikes.Next(Likes.Count)].UserId;
                User HHabLikes = ButterflyEnvironment.GetHabboById(Convert.ToInt32(useridLikes));
                this.WriteInteger(useridLikes); // UserId
                this.WriteString((HHabLikes == null) ? "" : HHabLikes.Username);
                this.WriteString((HHabLikes == null) ? "" : HHabLikes.Look); // look
            }
            if (Hates.Count > 0)
            {
                //Hates
                this.WriteInteger(3); //type
                this.WriteInteger(Hates.Count); //Total personne hates

                Random randHates = new Random();
                int useridHates = Hates.ElementAt(randHates.Next(0, Hates.Count)).Value.UserId;//Hates[randHates.Next(Hates.Count)].UserId;
                User HHabHates = ButterflyEnvironment.GetHabboById(Convert.ToInt32(useridHates));
                this.WriteInteger(useridHates); // UserId
                this.WriteString((HHabHates == null) ? "" : HHabHates.Username);
                this.WriteString((HHabHates == null) ? "" : HHabHates.Look); // look
            }
        }
    }
}
