using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetGroupMembersEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null)
            {
                return;
            }

            int GroupId = Packet.PopInt();
            int Page = Packet.PopInt();
            string SearchVal = Packet.PopString();
            int RequestType = Packet.PopInt();

            if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            int StartIndex = (Page - 1) * 14 + 14;
            List<User> Members = new List<User>();
            int MemberCount = 0;

            switch (RequestType)
            {
                case 0:
                    MemberCount = Group.GetAllMembers.Count();
                    List<int> MemberIds;
                    if (!string.IsNullOrEmpty(SearchVal))
                    {
                        MemberIds = this.GetSearchMembres(Group.Id, SearchVal);
                    }
                    else
                    {
                        MemberIds = Group.GetAllMembers.Skip(StartIndex).Take(14).ToList();
                    }

                    foreach (int Id in MemberIds.ToList())
                    {
                        User GroupMember = WibboEnvironment.GetUserById(Id);
                        if (GroupMember == null)
                        {
                            continue;
                        }

                        if (!Members.Contains(GroupMember))
                        {
                            Members.Add(GroupMember);
                        }
                    }
                    break;
                case 1:
                    MemberCount = Group.GetAdministrators.Count();
                    List<int> AdminIds;
                    if (!string.IsNullOrEmpty(SearchVal))
                    {
                        AdminIds = this.GetSearchAdmins(Group.Id, SearchVal);
                    }
                    else
                    {
                        AdminIds = Group.GetAdministrators.Skip(StartIndex).Take(14).ToList();
                    }

                    foreach (int User in AdminIds.ToList())
                    {
                        User GroupMember = WibboEnvironment.GetUserById(User);
                        if (GroupMember == null)
                        {
                            continue;
                        }

                        if (!Members.Contains(GroupMember))
                        {
                            Members.Add(GroupMember);
                        }
                    }
                    break;
                case 2:
                    MemberCount = Group.GetRequests.Count();
                    List<int> RequestIds;
                    if (!string.IsNullOrEmpty(SearchVal))
                    {
                        RequestIds = this.GetSearchRequests(Group.Id, SearchVal);
                    }
                    else
                    {
                        RequestIds = Group.GetRequests.Skip(StartIndex).Take(14).ToList();
                    }

                    foreach (int Id in RequestIds.ToList())
                    {
                        User GroupMember = WibboEnvironment.GetUserById(Id);
                        if (GroupMember == null)
                        {
                            continue;
                        }

                        if (!Members.Contains(GroupMember))
                        {
                            Members.Add(GroupMember);
                        }
                    }
                    break;
            }

            Session.SendPacket(new GroupMembersComposer(Group, Members.ToList(), MemberCount, Page, (Group.CreatorId == Session.GetUser().Id || Group.IsAdmin(Session.GetUser().Id)), RequestType, SearchVal));
        }

        private List<int> GetSearchRequests(int groupeId, string searchVal)
        {
            List<int> MembersId = new List<int>();

            DataTable MembresTable = null;
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                MembresTable = GuildRequestDao.GetAllBySearch(dbClient, groupeId, searchVal);

            foreach (DataRow row in MembresTable.Rows)
            {
                if (!MembersId.Contains(Convert.ToInt32(row["id"])))
                {
                    MembersId.Add(Convert.ToInt32(row["id"]));
                }
            }

            return MembersId;
        }

        private List<int> GetSearchAdmins(int GroupeId, string SearchVal)
        {
            List<int> MembersId = new List<int>();

            DataTable MembresTable = null;
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                MembresTable = GuildMembershipDao.GetAllUserIdBySearchAndStaff(dbClient, GroupeId, SearchVal);

            foreach (DataRow row in MembresTable.Rows)
            {
                if (!MembersId.Contains(Convert.ToInt32(row["id"])))
                {
                    MembersId.Add(Convert.ToInt32(row["id"]));
                }
            }

            return MembersId;
        }

        private List<int> GetSearchMembres(int GroupeId, string SearchVal)
        {
            List<int> MembersId = new List<int>();

            DataTable MembresTable = null;
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                MembresTable = GuildMembershipDao.GetAllUserIdBySearch(dbClient, GroupeId, SearchVal);
            }

            foreach (DataRow row in MembresTable.Rows)
            {
                if (!MembersId.Contains(Convert.ToInt32(row["id"])))
                {
                    MembersId.Add(Convert.ToInt32(row["id"]));
                }
            }

            return MembersId;
        }
    }
}