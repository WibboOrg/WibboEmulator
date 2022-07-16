using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Groups;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class ManageGroupEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();

            if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            if (Group.CreatorId != Session.GetUser().Id && !Session.GetUser().HasPermission("group_management_override"))
            {
                return;
            }

            Session.SendPacket(new ManageGroupComposer(Group, Group.Badge.Replace("b", "").Split('s')));
        }
    }
}