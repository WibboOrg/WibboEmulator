using Butterfly.Communication.Packets.Outgoing.Groups;

using Butterfly.Game.Clients;
using Butterfly.Game.Guilds;
using Butterfly.Game.User;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class AcceptGroupMembershipEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Guild Group))
            {
                return;
            }

            if ((Session.GetHabbo().Id != Group.CreatorId && !Group.IsAdmin(Session.GetHabbo().Id)) && !Session.GetHabbo().HasFuse("fuse_group_accept_any"))
            {
                return;
            }

            if (!Group.HasRequest(UserId))
            {
                return;
            }

            Habbo Habbo = ButterflyEnvironment.GetHabboById(UserId);
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