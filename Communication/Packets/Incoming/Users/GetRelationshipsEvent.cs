using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Game.Clients;
using Butterfly.Game.Users;
using Butterfly.Game.Users.Messenger;
using System.Collections.Generic;
using System.Linq;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetRelationshipsEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            User User = ButterflyEnvironment.GetHabboById(Packet.PopInt());
            if (User == null)
                return;

            ICollection<Relationship>relations = User.GetMessenger().GetRelationships();


            Session.SendPacket(new GetRelationshipsComposer(User.Id, relations));
        }
    }
}