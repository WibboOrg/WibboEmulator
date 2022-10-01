using WibboEmulator.Communication.Packets.Outgoing.Groups;

using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class DeclineGroupMembershipEvent : IPacketEvent
    {
        public double Delay => 100;

        public void Parse(GameClient Session, ClientPacket Packet)
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