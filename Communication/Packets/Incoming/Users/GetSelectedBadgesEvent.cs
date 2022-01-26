using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using Butterfly.Game.Users;
using Butterfly.Game.Users.Badges;

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

            Session.SendPacket(new HabboUserBadgesComposer(User));
        }
    }
}