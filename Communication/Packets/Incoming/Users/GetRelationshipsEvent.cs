using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Game.Clients;
using Butterfly.Game.Users;
using Butterfly.Game.Users.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetRelationshipsEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            int Id = Packet.PopInt();

            Client Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Id);

            if (Client == null || Client.GetHabbo() == null)
            {
                Session.SendPacket(new GetRelationshipsComposer(Id, 0, null, null, null));
                return;
            }

            User habbo = Client.GetHabbo();
            if (habbo == null || habbo.GetMessenger() == null)
            {
                Session.SendPacket(new GetRelationshipsComposer(Id, 0, null, null, null));
                return;
            }

            Dictionary<int, Relationship> Loves = habbo.GetMessenger().Relation.Where(x => x.Value.Type == 1).ToDictionary(item => item.Key, item => item.Value);
            Dictionary<int, Relationship> Likes = habbo.GetMessenger().Relation.Where(x => x.Value.Type == 2).ToDictionary(item => item.Key, item => item.Value);
            Dictionary<int, Relationship> Hates = habbo.GetMessenger().Relation.Where(x => x.Value.Type == 3).ToDictionary(item => item.Key, item => item.Value);
            int Nbrela = 0;
            if (Loves.Count > 0)
            {
                Nbrela++;
            }

            if (Likes.Count > 0)
            {
                Nbrela++;
            }

            if (Hates.Count > 0)
            {
                Nbrela++;
            }

            Session.SendPacket(new GetRelationshipsComposer(habbo.Id, Nbrela, Loves, Likes, Hates));
        }
        
    }
}