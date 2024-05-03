namespace WibboEmulator.Communication.Packets.Incoming.Groups;

using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Guild;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Users;

internal sealed class GetGroupMembersEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null)
        {
            return;
        }

        var groupId = packet.PopInt();
        var page = packet.PopInt();
        var searchVal = packet.PopString(16);
        var requestType = packet.PopInt();

        if (!GroupManager.TryGetGroup(groupId, out var group))
        {
            return;
        }

        var startIndex = ((page - 1) * 14) + 14;
        var members = new List<User>();
        var memberCount = 0;

        switch (requestType)
        {
            case 0:
                memberCount = group.GetAllMembers.Count;
                List<int> memberIds;
                if (!string.IsNullOrEmpty(searchVal))
                {
                    memberIds = GetSearchMembres(group.Id, searchVal);
                }
                else
                {
                    memberIds = group.GetAllMembers.Skip(startIndex).Take(14).ToList();
                }

                foreach (var id in memberIds.ToList())
                {
                    var groupMember = WibboEnvironment.GetUserById(id);
                    if (groupMember == null)
                    {
                        continue;
                    }

                    if (!members.Contains(groupMember))
                    {
                        members.Add(groupMember);
                    }
                }
                break;
            case 1:
                memberCount = group.GetAdministrators.Count;
                List<int> adminIds;
                if (!string.IsNullOrEmpty(searchVal))
                {
                    adminIds = GetSearchAdmins(group.Id, searchVal);
                }
                else
                {
                    adminIds = group.GetAdministrators.Skip(startIndex).Take(14).ToList();
                }

                foreach (var user in adminIds.ToList())
                {
                    var groupMember = WibboEnvironment.GetUserById(user);
                    if (groupMember == null)
                    {
                        continue;
                    }

                    if (!members.Contains(groupMember))
                    {
                        members.Add(groupMember);
                    }
                }
                break;
            case 2:
                memberCount = group.GetRequests.Count;
                List<int> requestIds;
                if (!string.IsNullOrEmpty(searchVal))
                {
                    requestIds = GetSearchRequests(group.Id, searchVal);
                }
                else
                {
                    requestIds = group.GetRequests.Skip(startIndex).Take(14).ToList();
                }

                foreach (var id in requestIds.ToList())
                {
                    var groupMember = WibboEnvironment.GetUserById(id);
                    if (groupMember == null)
                    {
                        continue;
                    }

                    if (!members.Contains(groupMember))
                    {
                        members.Add(groupMember);
                    }
                }
                break;
        }

        session.SendPacket(new GroupMembersComposer(group, [.. members], memberCount, page, group.CreatorId == session.User.Id || group.IsAdmin(session.User.Id), requestType, searchVal));
    }

    private static List<int> GetSearchRequests(int groupeId, string searchVal)
    {
        using var dbClient = DatabaseManager.Connection;
        var membersId = GuildRequestDao.GetAllBySearch(dbClient, groupeId, searchVal);

        return membersId;
    }

    private static List<int> GetSearchAdmins(int groupeId, string searchVal)
    {
        using var dbClient = DatabaseManager.Connection;
        var membersId = GuildMembershipDao.GetAllUserIdBySearchAndStaff(dbClient, groupeId, searchVal);

        return membersId;
    }

    private static List<int> GetSearchMembres(int groupeId, string searchVal)
    {
        using var dbClient = DatabaseManager.Connection;
        var membersId = GuildMembershipDao.GetAllUserIdBySearch(dbClient, groupeId, searchVal);

        return membersId;
    }
}
