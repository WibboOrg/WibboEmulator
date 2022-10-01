using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Users;
using WibboEmulator.Games.Users.Relationships;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetRelationshipsEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            User User = WibboEnvironment.GetUserById(Packet.PopInt());
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