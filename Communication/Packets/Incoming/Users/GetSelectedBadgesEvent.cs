using Wibbo.Communication.Packets.Outgoing.Users;
using Wibbo.Game.Clients;
using Wibbo.Game.Users;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class GetSelectedBadgesEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int UserId = Packet.PopInt();

            User User = WibboEnvironment.GetUserById(UserId);
            if (User == null)
                return;
            if (User.GetBadgeComponent() == null)
                return;

            Session.SendPacket(new UserBadgesComposer(User));
        }
    }
}