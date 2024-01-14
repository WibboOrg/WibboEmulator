namespace WibboEmulator.Games.Groups;

using WibboEmulator.Database.Daos.Guild;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool AdminOnlyDeco { get; set; }
    public string Badge { get; set; }
    public int CreateTime { get; set; }
    public int CreatorId { get; set; }
    public string Description { get; set; }
    public int RoomId { get; set; }
    public int Colour1 { get; set; }
    public int Colour2 { get; set; }
    public bool ForumEnabled { get; set; }
    public GroupType GroupType { get; set; }
    public bool HasForum { get; set; }

    private readonly List<int> _members;
    private readonly List<int> _requests;
    private readonly List<int> _administrators;

    public Group(int id, string name, string description, string badge, int roomId, int owner, int time, int type, int colour1, int colour2,
        bool adminOnlyDeco, bool hasForum)
    {
        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.RoomId = roomId;
        this.Badge = badge;
        this.CreateTime = time;
        this.CreatorId = owner;
        this.Colour1 = (colour1 == 0) ? 1 : colour1;
        this.Colour2 = (colour2 == 0) ? 1 : colour2;
        this.HasForum = hasForum;

        switch (type)
        {
            case 0:
                this.GroupType = GroupType.Open;
                break;
            case 1:
                this.GroupType = GroupType.Locked;
                break;
            case 2:
                this.GroupType = GroupType.Private;
                break;
        }

        this.AdminOnlyDeco = adminOnlyDeco;
        this.ForumEnabled = false;

        this._members = new List<int>();
        this._requests = new List<int>();
        this._administrators = new List<int>();

        this.InitMembers();
    }

    public void InitMembers()
    {
        using var dbClient = WibboEnvironment.GetDatabaseManager().Connection();

        var guildMembershipList = GuildMembershipDao.GetAll(dbClient, this.Id);

        if (guildMembershipList.Count != 0)
        {
            foreach (var guildMembership in guildMembershipList)
            {
                var userId = guildMembership.UserId;
                var isAdmin = guildMembership.Rank != 0;

                if (isAdmin)
                {
                    if (!this._administrators.Contains(userId))
                    {
                        this._administrators.Add(userId);
                    }
                }
                else
                {
                    if (!this._members.Contains(userId))
                    {
                        this._members.Add(userId);
                    }
                }
            }
        }

        var requestIdList = GuildRequestDao.GetAll(dbClient, this.Id);

        if (requestIdList.Count != 0)
        {
            foreach (var userId in requestIdList)
            {
                if (this._members.Contains(userId) || this._administrators.Contains(userId))
                {
                    GuildRequestDao.Delete(dbClient, this.Id, userId);
                }
                else if (!this._requests.Contains(userId))
                {
                    this._requests.Add(userId);
                }
            }
        }
    }

    public List<int> GetMembers => this._members.ToList();

    public List<int> GetRequests => this._requests.ToList();

    public List<int> GetAdministrators => this._administrators.ToList();

    public List<int> GetAllMembers
    {
        get
        {
            var members = new List<int>(this._administrators.ToList());
            members.AddRange(this._members.ToList());

            return members;
        }
    }

    public int MemberCount => this._members.Count + this._administrators.Count;

    public int RequestCount => this._requests.Count;

    public bool IsMember(int id) => this._members.Contains(id) || this._administrators.Contains(id);

    public bool IsAdmin(int id) => this._administrators.Contains(id);

    public bool HasRequest(int id) => this._requests.Contains(id);

    public void MakeAdmin(int userId)
    {
        if (this._members.Contains(userId))
        {
            _ = this._members.Remove(userId);
        }

        if (this._administrators.Contains(userId))
        {
            return;
        }

        this._administrators.Add(userId);

        using var dbClient = WibboEnvironment.GetDatabaseManager().Connection();
        GuildMembershipDao.UpdateRank(dbClient, this.Id, userId, 1);
    }

    public void TakeAdmin(int userId)
    {
        if (!this._administrators.Contains(userId))
        {
            return;
        }

        _ = this._administrators.Remove(userId);

        if (!this._members.Contains(userId))
        {
            this._members.Add(userId);
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().Connection();
        GuildMembershipDao.UpdateRank(dbClient, this.Id, userId, 0);
    }

    public void AddMember(int userId)
    {
        using var dbClient = WibboEnvironment.GetDatabaseManager().Connection();
        if (this.IsAdmin(userId))
        {
            GuildMembershipDao.UpdateRank(dbClient, this.Id, userId, 0);
            _ = this._administrators.Remove(userId);
            this._members.Add(userId);
        }
        else if (this.GroupType == GroupType.Locked)
        {
            GuildRequestDao.Insert(dbClient, this.Id, userId);
            if (!this._requests.Contains(userId))
            {
                this._requests.Add(userId);
            }
        }
        else
        {
            GuildMembershipDao.Insert(dbClient, this.Id, userId);
            if (!this._members.Contains(userId))
            {
                this._members.Add(userId);
            }
        }
    }

    public void DeleteMember(int userId)
    {
        if (this.IsMember(userId))
        {
            if (this._members.Contains(userId))
            {
                _ = this._members.Remove(userId);
            }
        }
        else if (this.IsAdmin(userId))
        {
            if (this._administrators.Contains(userId))
            {
                _ = this._administrators.Remove(userId);
            }
        }
        else
        {
            return;
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().Connection();
        GuildMembershipDao.Delete(dbClient, this.Id, userId);
    }

    public void HandleRequest(int userId, bool accepted)
    {
        using (var dbClient = WibboEnvironment.GetDatabaseManager().Connection())
        {
            if (accepted)
            {
                GuildMembershipDao.Insert(dbClient, this.Id, userId);

                if (!this._members.Contains(userId))
                {
                    this._members.Add(userId);
                }
            }

            GuildRequestDao.Delete(dbClient, this.Id, userId);
        }

        if (this._requests.Contains(userId))
        {
            _ = this._requests.Remove(userId);
        }
    }

    public void ClearRequests() => this._requests.Clear();

    public void Dispose()
    {
        this._requests.Clear();
        this._members.Clear();
        this._administrators.Clear();
    }
}
