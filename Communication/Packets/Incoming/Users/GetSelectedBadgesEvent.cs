using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Game.Clients;
using Butterfly.Game.Users;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetSelectedBadgesEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            int UserId = Packet.PopInt();

            User User = ButterflyEnvironment.GetHabboById(UserId);
            if (User == null)
                return;
            if (User.GetBadgeComponent() == null)
                return;

            Session.SendPacket(new HabboUserBadgesComposer(User));
        }
    }
}