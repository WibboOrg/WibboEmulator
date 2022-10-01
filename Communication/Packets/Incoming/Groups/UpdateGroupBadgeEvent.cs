using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Groups;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class UpdateGroupBadgeEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();

            if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            if (Group.CreatorId != Session.GetUser().Id)
            {
                return;
            }

            int Count = Packet.PopInt();

            string Badge = "";
            for (int i = 0; i < Count; i++)
            {
                Badge += BadgePartUtility.WorkBadgeParts(i == 0, Packet.PopInt().ToString(), Packet.PopInt().ToString(), Packet.PopInt().ToString());
            }

            Group.Badge = (string.IsNullOrWhiteSpace(Badge) ? "b05114s06114" : Badge);

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                GuildDao.UpdateBadge(dbClient, Group.Id, Group.Badge);
            }

            Session.SendPacket(new GroupInfoComposer(Group, Session));
        }
    }
}
