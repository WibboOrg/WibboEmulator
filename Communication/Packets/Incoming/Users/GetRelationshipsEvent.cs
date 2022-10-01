using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.GameClients.Relationships;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetRelationshipsEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet)
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