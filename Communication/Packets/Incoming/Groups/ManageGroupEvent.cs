using Butterfly.Communication.Packets.Outgoing.Groups;
using Butterfly.Game.Clients;
using Butterfly.Game.Guilds;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ManageGroupEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();

            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            if (Group.CreatorId != Session.GetHabbo().Id && !Session.GetHabbo().HasFuse("group_management_override"))
            {
                return;
            }

            Session.SendPacket(new ManageGroupComposer(Group, Group.Badge.Replace("b", "").Split('s')));
        }
    }
}