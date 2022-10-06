namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Database.Daos.Guild;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Users;

internal class GetGroupMembersEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session == null || session.GetUser() == null)
        {
            return;
        }

        var GroupId = Packet.PopInt();
        var Page = Packet.PopInt();
        var SearchVal = Packet.PopString();
        var RequestType = Packet.PopInt();

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out var Group))
        {
            return;
        }

        var StartIndex = (Page - 1) * 14 + 14;
        var Members = new List<User>();
        var MemberCount = 0;

        switch (RequestType)
        {
            case 0:
                MemberCount = Group.GetAllMembers.Count;
                List<int> MemberIds;
                if (!string.IsNullOrEmpty(SearchVal))
                {
                    MemberIds = GetSearchMembres(Group.Id, SearchVal);
                }
                else
                {
                    MemberIds = Group.GetAllMembers.Skip(StartIndex).Take(14).ToList();
                }

                foreach (var Id in MemberIds.ToList())
                {
                    var GroupMember = WibboEnvironment.GetUserById(Id);
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
                MemberCount = Group.GetAdministrators.Count;
                List<int> AdminIds;
                if (!string.IsNullOrEmpty(SearchVal))
                {
                    AdminIds = GetSearchAdmins(Group.Id, SearchVal);
                }
                else
                {
                    AdminIds = Group.GetAdministrators.Skip(StartIndex).Take(14).ToList();
                }

                foreach (var User in AdminIds.ToList())
                {
                    var GroupMember = WibboEnvironment.GetUserById(User);
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
                MemberCount = Group.GetRequests.Count;
                List<int> requestIds;
                if (!string.IsNullOrEmpty(SearchVal))
                {
                    requestIds = GetSearchRequests(Group.Id, SearchVal);
                }
                else
                {
                    requestIds = Group.GetRequests.Skip(StartIndex).Take(14).ToList();
                }

                foreach (var id in requestIds.ToList())
                {
                    var groupMember = WibboEnvironment.GetUserById(id);
                    if (groupMember == null)
                    {
                        continue;
                    }

                    if (!Members.Contains(groupMember))
                    {
                        Members.Add(groupMember);
                    }
                }
                break;
        }

        session.SendPacket(new GroupMembersComposer(Group, Members.ToList(), MemberCount, Page, Group.CreatorId == session.GetUser().Id || Group.IsAdmin(session.GetUser().Id), RequestType, SearchVal));
    }

    private static List<int> GetSearchRequests(int groupeId, string searchVal)
    {
        var membersId = new List<int>();

        DataTable membresTable = null;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            membresTable = GuildRequestDao.GetAllBySearch(dbClient, groupeId, searchVal);
        }

        foreach (DataRow row in membresTable.Rows)
        {
            if (!membersId.Contains(Convert.ToInt32(row["id"])))
            {
                membersId.Add(Convert.ToInt32(row["id"]));
            }
        }

        return membersId;
    }

    private static List<int> GetSearchAdmins(int groupeId, string searchVal)
    {
        var membersId = new List<int>();

        DataTable membresTable = null;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            membresTable = GuildMembershipDao.GetAllUserIdBySearchAndStaff(dbClient, groupeId, searchVal);
        }

        foreach (DataRow row in membresTable.Rows)
        {
            if (!membersId.Contains(Convert.ToInt32(row["id"])))
            {
                membersId.Add(Convert.ToInt32(row["id"]));
            }
        }

        return membersId;
    }

    private static List<int> GetSearchMembres(int groupeId, string searchVal)
    {
        var membersId = new List<int>();

        DataTable membresTable = null;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            membresTable = GuildMembershipDao.GetAllUserIdBySearch(dbClient, groupeId, searchVal);
        }

        foreach (DataRow row in membresTable.Rows)
        {
            if (!membersId.Contains(Convert.ToInt32(row["id"])))
            {
                membersId.Add(Convert.ToInt32(row["id"]));
            }
        }

        return membersId;
    }
}
