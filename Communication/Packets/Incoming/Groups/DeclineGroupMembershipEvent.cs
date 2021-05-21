using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Groups;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class DeclineGroupMembershipEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            if (Session.GetHabbo().Id != Group.CreatorId && !Group.IsAdmin(Session.GetHabbo().Id))
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