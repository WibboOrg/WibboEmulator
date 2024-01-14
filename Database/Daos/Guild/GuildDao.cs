namespace WibboEmulator.Database.Daos.Guild;
using System.Data;
using Dapper;

internal sealed class GuildDao
{
    internal static void UpdateBadge(IDbConnection dbClient, int groupId, string badge) => dbClient.Execute(
        "UPDATE guild SET badge = @Badge WHERE id = @GroupId LIMIT 1",
        new { Badge = badge, GroupId = groupId });

    internal static void UpdateColors(IDbConnection dbClient, int colour1, int colour2, int groupId) => dbClient.Execute(
        "UPDATE guild SET colour1 = @Colour1, colour2 = @Colour2 WHERE id = @GroupId LIMIT 1",
        new { Colour1 = colour1, Colour2 = colour2, GroupId = groupId });

    internal static void UpdateNameAndDesc(IDbConnection dbClient, int groupId, string name, string desc) => dbClient.Execute(
        "UPDATE guild SET name = @Name, `desc` = @Desc WHERE id = @GroupId LIMIT 1",
        new { Name = name, Desc = desc, GroupId = groupId });

    internal static void UpdateStateAndDeco(IDbConnection dbClient, int groupId, int groupState, int furniOptions) => dbClient.Execute(
        "UPDATE guild SET state = @GroupState, admindeco = @AdminDeco WHERE id = @GroupId LIMIT 1",
        new { GroupState = groupState, AdminDeco = furniOptions, GroupId = groupId });

    internal static void Delete(IDbConnection dbClient, int groupId) => dbClient.Execute(
        "DELETE FROM `guild` WHERE `id` = @GroupId",
        new { GroupId = groupId });

    internal static GuildEntity GetOne(IDbConnection dbClient, int groupId) => dbClient.QuerySingleOrDefault<GuildEntity>(
        @"SELECT `id`, `name`, `desc`, `badge`, `owner_id`, `created`, `room_id`, `state`, `colour1`, `colour2`, `admindeco`, `has_forum` 
        FROM `guild` 
        WHERE `id` = @Id LIMIT 1",
        new { Id = groupId });

    internal static int Insert(IDbConnection dbClient, string name, string description, int creatorId, string badge, int roomId, int colour1, int colour2) => dbClient.ExecuteScalar<int>(
        @"INSERT INTO `guild` (`name`, `desc`, `badge`, `owner_id`, `created`, `room_id`, `state`, `colour1`, `colour2`, `admindeco`) 
        VALUES (@name, @desc, @badge, @owner, UNIX_TIMESTAMP(), @room, '0', @colour1, @colour2, '1'); 
        SELECT LAST_INSERT_ID();",
        new { name, desc = description, owner = creatorId, badge, room = roomId, colour1, colour2 });
}

public class GuildEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Desc { get; set; }
    public string Badge { get; set; }
    public int OwnerId { get; set; }
    public int Created { get; set; }
    public int RoomId { get; set; }
    public int State { get; set; }
    public int Colour1 { get; set; }
    public int Colour2 { get; set; }
    public bool AdminDeco { get; set; }
    public bool HasForum { get; set; }
}