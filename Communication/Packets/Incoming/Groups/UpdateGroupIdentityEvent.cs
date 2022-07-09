using Wibbo.Communication.Packets.Outgoing.Groups;
using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Clients;
using Wibbo.Game.Groups;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class UpdateGroupIdentityEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            string Name = WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());
            string Desc = WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());

            if (Name.Length > 50)
            {
                return;
            }

            if (Desc.Length > 255)
            {
                return;
            }

            if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            if (Group.CreatorId != Session.GetUser().Id)
            {
                return;
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                GuildDao.UpdateNameAndDesc(dbClient, GroupId, Name, Desc);
            }

            Group.Name = Name;
            Group.Description = Desc;

            Session.SendPacket(new GroupInfoComposer(Group, Session));

        }
    }
}
