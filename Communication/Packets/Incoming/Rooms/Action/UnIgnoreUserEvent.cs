using Butterfly.Communication.Packets.Outgoing.Rooms.Action;
using Butterfly.Game.Clients;
using Butterfly.Game.Users;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class UnIgnoreUserEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
            {
                return;
            }

            if (Session.GetHabbo().CurrentRoom == null)
            {
                return;
            }

            string str = Packet.PopString();

            User habbo = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(str).GetHabbo();
            if (habbo == null || !Session.GetHabbo().MutedUsers.Contains(habbo.Id))
            {
                return;
            }

            Session.GetHabbo().MutedUsers.Remove(habbo.Id);

            Session.SendPacket(new IgnoreStatusComposer(3, str));
        }
    }
}
