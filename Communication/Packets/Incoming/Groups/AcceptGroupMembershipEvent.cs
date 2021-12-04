using Butterfly.Communication.Packets.Outgoing.Groups;

using Butterfly.Game.Clients;
using Butterfly.Game.Guilds;
using Butterfly.Game.Users;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class AcceptGroupMembershipEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            if ((Session.GetHabbo().Id != Group.CreatorId && !Group.IsAdmin(Session.GetHabbo().Id)) && !Session.GetHabbo().HasFuse("group_delete_limit_override"))
            {
                return;
            }

            if (!Group.HasRequest(UserId))
            {
                return;
            }

            User Habbo = ButterflyEnvironment.GetHabboById(UserId);
            if (Habbo == null)
            {
                return;
            }

            Group.HandleRequest(UserId, true);

            Habbo.MyGroups.Add(Group.Id);

            Session.SendPacket(new GroupMemberUpdatedComposer(GroupId, Habbo, 4));
        }
    }
}