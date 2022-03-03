using Butterfly.Communication.Packets.Outgoing.Groups;

using Butterfly.Game.Clients;
using Butterfly.Game.Guilds;
using Butterfly.Game.Users;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class AcceptGroupMembershipEvent : IPacketEvent
    {
        public double Delay => 100;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            if ((Session.GetUser().Id != Group.CreatorId && !Group.IsAdmin(Session.GetUser().Id)) && !Session.GetUser().HasFuse("group_delete_limit_override"))
            {
                return;
            }

            if (!Group.HasRequest(UserId))
            {
                return;
            }

            User user = ButterflyEnvironment.GetUserById(UserId);
            if (user == null)
            {
                return;
            }

            Group.HandleRequest(UserId, true);

            user.MyGroups.Add(Group.Id);

            Session.SendPacket(new GroupMemberUpdatedComposer(GroupId, user, 4));
        }
    }
}