namespace WibboEmulator.Games.Groups;
using System.Collections.Concurrent;
using System.Data;
using WibboEmulator.Database.Daos.Guild;
using WibboEmulator.Database.Daos.Room;
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

    public void Initialize(IDbConnection dbClient)
    {
        this._bases.Clear();
        this._symbols.Clear();
        this._baseColours.Clear();
        this._symbolColours.Clear();
        this._backgroundColours.Clear();

        var guildItemList = GuildItemDao.GetAll(dbClient);

        if (guildItemList.Count == 0)
        {
            return;
        }

        foreach (var guildItem in guildItemList)
        {
            switch (guildItem.Type)
            {
                case GuildItemType.Base:
                    this._bases.Add(new GroupBadgeParts(guildItem.Id, guildItem.FirstValue, guildItem.SecondValue));
                    break;

                case GuildItemType.Symbol:
                    this._symbols.Add(new GroupBadgeParts(guildItem.Id, guildItem.FirstValue, guildItem.SecondValue));
                    break;

                case GuildItemType.Color:
                    this._baseColours.Add(new GroupColours(guildItem.Id, guildItem.FirstValue));
                    break;

                case GuildItemType.Color2:
                    this._symbolColours.Add(guildItem.Id, new GroupColours(guildItem.Id, guildItem.FirstValue));
                    break;

                case GuildItemType.Color3:
                    this._backgroundColours.Add(guildItem.Id, new GroupColours(guildItem.Id, guildItem.FirstValue));
                    break;
            }
        }
    }

    public bool TryGetGroup(int id, out Group group)
    {
        if (id == 0)
        {
            group = null;
            return false;
        }

        if (this._groups.ContainsKey(id))
        {
            return this._groups.TryGetValue(id, out group);
        }

        lock (this._groupLoadingSync)
        {
            using var dbClient = WibboEnvironment.GetDatabaseManager().Connection();

            var guild = GuildDao.GetOne(dbClient, id);

            if (guild == null)
            {
                group = null;
                return false;
            }

            group = new Group(
                    guild.Id, guild.Name, guild.Desc, guild.Badge, guild.RoomId, guild.OwnerId,
                    guild.Created, guild.State, guild.Colour1, guild.Colour2, guild.AdminDeco, guild.HasForum);

            _ = this._groups.TryAdd(group.Id, group);

            return true;
        }
    }

    public bool TryCreateGroup(User user, string name, string description, int roomId, string badge, int colour1, int colour2, out Group group)
    {
        group = new Group(0, name, description, badge, roomId, user.Id, WibboEnvironment.GetUnixTimestamp(), 0, colour1, colour2, false, false);
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(badge))
        {
            return false;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().Connection())
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
            if (this._symbolColours.TryGetValue(id, out var value))
            {
                return value.Colour;
            }
        }
        else
        {
            if (this._backgroundColours.TryGetValue(id, out var value))
            {
                return value.Colour;
            }
        }

        return "";
    }

    public void DeleteGroup(int id)
    {
        if (this._groups.TryGetValue(id, out var group))
        {
            group.Dispose();
            _ = this._groups.TryRemove(id, out _);
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
