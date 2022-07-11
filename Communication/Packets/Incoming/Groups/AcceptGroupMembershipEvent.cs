using WibboEmulator.Communication.Packets.Outgoing.Groups;

using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Groups;
using WibboEmulator.Game.Users;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class AcceptGroupMembershipEvent : IPacketEvent
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

            if ((Session.GetUser().Id != Group.CreatorId && !Group.IsAdmin(Session.GetUser().Id)) && !Session.GetUser().HasFuse("group_delete_limit_override"))
            {
                return;
            }

            if (!Group.HasRequest(UserId))
            {
                return;
            }

            User user = WibboEnvironment.GetUserById(UserId);
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