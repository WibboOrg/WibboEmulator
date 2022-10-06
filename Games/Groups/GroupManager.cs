namespace WibboEmulator.Games.Groups;
using System.Collections.Concurrent;
using System.Data;
using WibboEmulator.Database.Daos.Guild;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Users;

public class GroupManager
{
    private readonly object _groupLoadingSync;

    private readonly ConcurrentDictionary<int, Group> _groups;

    private readonly List<GroupBadgeParts> _bases;
    private readonly List<GroupBadgeParts> _symbols;
    private readonly List<GroupColours> _baseColours;
    private readonly Dictionary<int, GroupColours> _symbolColours;
    private readonly Dictionary<int, GroupColours> _backgroundColours;

    public GroupManager()
    {
        this._groupLoadingSync = new object();

        this._groups = new ConcurrentDictionary<int, Group>();

        this._bases = new List<GroupBadgeParts>();
        this._symbols = new List<GroupBadgeParts>();
        this._baseColours = new List<GroupColours>();
        this._symbolColours = new Dictionary<int, GroupColours>();
        this._backgroundColours = new Dictionary<int, GroupColours>();
    }

    public void Init(IQueryAdapter dbClient)
    {
        this._bases.Clear();
        this._symbols.Clear();
        this._baseColours.Clear();
        this._symbolColours.Clear();
        this._backgroundColours.Clear();

        var dItems = GuildItemDao.GetAll(dbClient);

        foreach (DataRow dRow in dItems.Rows)
        {
            switch (dRow["type"].ToString())
            {
                case "base":
                    this._bases.Add(new GroupBadgeParts(Convert.ToInt32(dRow["id"]), dRow["firstvalue"].ToString(), dRow["secondvalue"].ToString()));
                    break;

                case "symbol":
                    this._symbols.Add(new GroupBadgeParts(Convert.ToInt32(dRow["id"]), dRow["firstvalue"].ToString(), dRow["secondvalue"].ToString()));
                    break;

                case "color":
                    this._baseColours.Add(new GroupColours(Convert.ToInt32(dRow["id"]), dRow["firstvalue"].ToString()));
                    break;

                case "color2":
                    this._symbolColours.Add(Convert.ToInt32(dRow["id"]), new GroupColours(Convert.ToInt32(dRow["id"]), dRow["firstvalue"].ToString()));
                    break;

                case "color3":
                    this._backgroundColours.Add(Convert.ToInt32(dRow["id"]), new GroupColours(Convert.ToInt32(dRow["id"]), dRow["firstvalue"].ToString()));
                    break;
            }
        }
    }

    public bool TryGetGroup(int id, out Group group)
    {
        group = null;

        if (this._groups.ContainsKey(id))
        {
            return this._groups.TryGetValue(id, out group);
        }

        lock (this._groupLoadingSync)
        {
            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                var row = GuildDao.GetOne(dbClient, id);

                if (row == null)
                {
                    return false;
                }

                group = new Group(
                        Convert.ToInt32(row["id"]), Convert.ToString(row["name"]), Convert.ToString(row["desc"]), Convert.ToString(row["badge"]), Convert.ToInt32(row["room_id"]), Convert.ToInt32(row["owner_id"]),
                        Convert.ToInt32(row["created"]), Convert.ToInt32(row["state"]), Convert.ToInt32(row["colour1"]), Convert.ToInt32(row["colour2"]), Convert.ToInt32(row["admindeco"]), Convert.ToInt32(row["has_forum"]) == 1);

                _ = this._groups.TryAdd(group.Id, group);
            }

            return true;
        }
    }

    public bool TryCreateGroup(User user, string name, string description, int roomId, string badge, int colour1, int colour2, out Group group)
    {
        group = new Group(0, name, description, badge, roomId, user.Id, WibboEnvironment.GetUnixTimestamp(), 0, colour1, colour2, 0, false);
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(badge))
        {
            return false;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            group.Id = GuildDao.Insert(dbClient, group.Name, group.Description, group.CreatorId, group.Badge, group.RoomId, group.Colour1, group.Colour2);

            group.AddMember(user.Id);
            group.MakeAdmin(user.Id);

            user.MyGroups.Add(group.Id);

            if (!this._groups.TryAdd(group.Id, group))
            {
                return false;
            }
            else
            {
                RoomDao.UpdateGroupId(dbClient, group.Id, group.RoomId);
                RoomRightDao.Delete(dbClient, roomId);
            }
        }
        return true;
    }

    public string GetColourCode(int id, bool colourOne)
    {
        if (colourOne)
        {
            if (this._symbolColours.ContainsKey(id))
            {
                return this._symbolColours[id].Colour;
            }
        }
        else
        {
            if (this._backgroundColours.ContainsKey(id))
            {
                return this._backgroundColours[id].Colour;
            }
        }

        return "";
    }

    public void DeleteGroup(int id)
    {
        Group group = null;
        if (this._groups.ContainsKey(id))
        {
            _ = this._groups.TryRemove(id, out _);
        }

        if (group != null)
        {
            group.Dispose();
        }
    }

    public List<Group> GetGroupsForUser(List<int> groupIds)
    {
        var groups = new List<Group>();

        foreach (var id in groupIds)
        {
            if (this.TryGetGroup(id, out var group))
            {
                groups.Add(group);
            }
        }
        return groups;
    }


    public ICollection<GroupBadgeParts> BadgeBases => this._bases;

    public ICollection<GroupBadgeParts> BadgeSymbols => this._symbols;

    public ICollection<GroupColours> BadgeBaseColours => this._baseColours;

    public ICollection<GroupColours> BadgeSymbolColours => this._symbolColours.Values;

    public ICollection<GroupColours> BadgeBackColours => this._backgroundColours.Values;
}
