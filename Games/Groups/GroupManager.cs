namespace WibboEmulator.Games.Groups;
using System.Collections.Concurrent;
using System.Data;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Guild;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.Users;

public static class GroupManager
{
    private static readonly object GroupLoadingSync = new();

    private static readonly ConcurrentDictionary<int, Group> Groups = new();

    private static readonly List<GroupBadgeParts> Bases = [];
    private static readonly List<GroupBadgeParts> Symbols = [];
    private static readonly List<GroupColours> BaseColours = [];
    private static readonly Dictionary<int, GroupColours> SymbolColours = [];
    private static readonly Dictionary<int, GroupColours> BackgroundColours = [];

    public static void Initialize(IDbConnection dbClient)
    {
        Bases.Clear();
        Symbols.Clear();
        BaseColours.Clear();
        SymbolColours.Clear();
        BackgroundColours.Clear();

        var guildItemList = GuildItemDao.GetAll(dbClient);

        foreach (var guildItem in guildItemList)
        {
            switch (guildItem.Type)
            {
                case GuildItemType.Base:
                    Bases.Add(new GroupBadgeParts(guildItem.Id, guildItem.FirstValue, guildItem.SecondValue));
                    break;

                case GuildItemType.Symbol:
                    Symbols.Add(new GroupBadgeParts(guildItem.Id, guildItem.FirstValue, guildItem.SecondValue));
                    break;

                case GuildItemType.Color:
                    BaseColours.Add(new GroupColours(guildItem.Id, guildItem.FirstValue));
                    break;

                case GuildItemType.Color2:
                    SymbolColours.Add(guildItem.Id, new GroupColours(guildItem.Id, guildItem.FirstValue));
                    break;

                case GuildItemType.Color3:
                    BackgroundColours.Add(guildItem.Id, new GroupColours(guildItem.Id, guildItem.FirstValue));
                    break;
            }
        }
    }

    public static bool TryGetGroup(int id, out Group group)
    {
        if (id == 0)
        {
            group = null;
            return false;
        }

        if (Groups.ContainsKey(id))
        {
            return Groups.TryGetValue(id, out group);
        }

        lock (GroupLoadingSync)
        {
            using var dbClient = DatabaseManager.Connection;

            var guild = GuildDao.GetOne(dbClient, id);

            if (guild == null)
            {
                group = null;
                return false;
            }

            group = new Group(
                    guild.Id, guild.Name, guild.Desc, guild.Badge, guild.RoomId, guild.OwnerId,
                    guild.Created, guild.State, guild.Colour1, guild.Colour2, guild.AdminDeco, guild.HasForum);

            _ = Groups.TryAdd(group.Id, group);

            return true;
        }
    }

    public static bool TryCreateGroup(User user, string name, string description, int roomId, string badge, int colour1, int colour2, out Group group)
    {
        group = new Group(0, name, description, badge, roomId, user.Id, WibboEnvironment.GetUnixTimestamp(), 0, colour1, colour2, false, false);
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(badge))
        {
            return false;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            group.Id = GuildDao.Insert(dbClient, group.Name, group.Description, group.CreatorId, group.Badge, group.RoomId, group.Colour1, group.Colour2);

            group.AddMember(user.Id);
            group.MakeAdmin(user.Id);

            user.MyGroups.Add(group.Id);

            if (!Groups.TryAdd(group.Id, group))
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

    public static string GetColourCode(int id, bool colourOne)
    {
        if (colourOne)
        {
            if (SymbolColours.TryGetValue(id, out var value))
            {
                return value.Colour;
            }
        }
        else
        {
            if (BackgroundColours.TryGetValue(id, out var value))
            {
                return value.Colour;
            }
        }

        return "";
    }

    public static void DeleteGroup(int id)
    {
        if (Groups.TryGetValue(id, out var group))
        {
            group.Dispose();
            _ = Groups.TryRemove(id, out _);
        }

    }

    public static List<Group> GetGroupsForUser(List<int> groupIds)
    {
        var groups = new List<Group>();

        foreach (var id in groupIds)
        {
            if (TryGetGroup(id, out var group))
            {
                groups.Add(group);
            }
        }
        return groups;
    }


    public static ICollection<GroupBadgeParts> BadgeBases => Bases;

    public static ICollection<GroupBadgeParts> BadgeSymbols => Symbols;

    public static ICollection<GroupColours> BadgeBaseColours => BaseColours;

    public static ICollection<GroupColours> BadgeSymbolColours => SymbolColours.Values;

    public static ICollection<GroupColours> BadgeBackColours => BackgroundColours.Values;
}
