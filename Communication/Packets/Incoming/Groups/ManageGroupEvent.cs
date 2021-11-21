using Butterfly.Communication.Packets.Outgoing.Groups;
using Butterfly.Game.GameClients;
using Butterfly.Game.Guilds;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ManageGroupEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();

            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Guild Group))
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