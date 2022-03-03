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
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            User User = ButterflyEnvironment.GetUserById(Packet.PopInt());
            if (User == null)
                return;

            if (User == null || User.GetMessenger() == null)
            {
                Session.SendPacket(new GetRelationshipsComposer(User.Id, new List<Relationship>()));
                return;
            }

            Session.SendPacket(new GetRelationshipsComposer(User.Id, User.GetMessenger().GetRelationships()));
        }
    }
}