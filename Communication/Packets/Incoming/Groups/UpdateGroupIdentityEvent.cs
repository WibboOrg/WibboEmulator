using Butterfly.Communication.Packets.Outgoing.Groups;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Guilds;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class UpdateGroupIdentityEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            string Name = ButterflyEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());
            string Desc = ButterflyEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());

            if (Name.Length > 50)
            {
                return;
            }

            if (Desc.Length > 255)
            {
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            if (Group.CreatorId != Session.GetHabbo().Id)
            {
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                GuildDao.UpdateNameAndDesc(dbClient, GroupId, Name, Desc);
            }

            Group.Name = Name;
            Group.Description = Desc;

            Session.SendPacket(new GroupInfoComposer(Group, Session));

        }
    }
}
