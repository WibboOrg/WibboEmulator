using Butterfly.Communication.Packets.Outgoing.Groups;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Groups;
using Butterfly.HabboHotel.Users;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetGroupMembersEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            int GroupId = Packet.PopInt();
            int Page = Packet.PopInt();
            string SearchVal = Packet.PopString();
            int RequestType = Packet.PopInt();

            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            int StartIndex = (Page - 1) * 14 + 14;
            List<Habbo> Members = new List<Habbo>();
            int MemberCount = 0;

            switch (RequestType)
            {
                case 0:
                    MemberCount = Group.GetAllMembers.Count();
                    List<int> MemberIds = null;
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
                        Habbo GroupMember = ButterflyEnvironment.GetHabboById(Id);
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
                    List<int> AdminIds = null;
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
                        Habbo GroupMember = ButterflyEnvironment.GetHabboById(User);
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
                    List<int> RequestIds = null;
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
                        Habbo GroupMember = ButterflyEnvironment.GetHabboById(Id);
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

            //if (!string.IsNullOrEmpty(SearchVal))
            //{
            //Members = Members.Where(x => x.Username.StartsWith(SearchVal)).ToList();
            //}

            Session.SendPacket(new GroupMembersComposer(Group, Members.ToList(), MemberCount, Page, (Group.CreatorId == Session.GetHabbo().Id || Group.IsAdmin(Session.GetHabbo().Id)), RequestType, SearchVal));

        }

        private List<int> GetSearchRequests(int groupeId, string searchVal)
        {
            List<int> MembersId = new List<int>();

            DataTable MembresTable = null;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                MembresTable = GuildRequestDao.GetAll(dbClient, groupeId, searchVal);

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
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
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
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
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