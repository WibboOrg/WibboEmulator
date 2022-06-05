using Wibbo.Communication.Packets.Outgoing.Groups;

using Wibbo.Game.Clients;
using Wibbo.Game.Groups;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class DeclineGroupMembershipEvent : IPacketEvent
    {
        public double Delay => 100;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            if (Session.GetUser().Id != Group.CreatorId && !Group.IsAdmin(Session.GetUser().Id))
            {
                return;
            }

            if (!Group.HasRequest(UserId))
            {
                return;
            }

            Group.HandleRequest(UserId, false);
            Session.SendPacket(new UnknownGroupComposer(Group.Id, UserId));
        }
    }
}