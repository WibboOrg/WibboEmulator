namespace WibboEmulator.Database.Daos.Room;
using System.Data;
using Dapper;

internal sealed class RoomDao
{
    internal static void UpdateResetGroupId(IDbConnection dbClient, int id) => dbClient.Execute(
        "UPDATE `room` SET group_id = '0' WHERE id = '" + id + "' LIMIT 1");

    internal static void UpdateGroupId(IDbConnection dbClient, int groupId, int roomId) => dbClient.Execute(
        "UPDATE room SET group_id = @GroupId WHERE id = @RoomId LIMIT 1",
        new { GroupId = groupId, RoomId = roomId });

    internal static void UpdateWiredSecurity(IDbConnection dbClient, int roomId, bool enabled) => dbClient.Execute(
        "UPDATE room SET wired_security = @Enabled WHERE id = @RoomId LIMIT 1",
        new { Enabled = enabled ? 1 : 0, RoomId = roomId });

    internal static void UpdateScore(IDbConnection dbClient, int roomId, int score) => dbClient.Execute(
        "UPDATE `room` SET score = '" + score + "' WHERE id = '" + roomId + "'");

    internal static void UpdateDecoration(IDbConnection dbClient, int roomId, string decorationKey, string extraData) => dbClient.Execute(
        "UPDATE room SET " + decorationKey + " = @ExtraData WHERE id = @RoomId LIMIT 1",
        new { ExtraData = extraData, RoomId = roomId });

    internal static void UpdateModelWallThickFloorThick(IDbConnection dbClient, int roomId, int wallThick, int floorThick) => dbClient.Execute(
        "UPDATE `room` SET model_name = 'model_custom', wallthick = '" + wallThick + "', floorthick = '" + floorThick + "' WHERE id = " + roomId + " LIMIT 1");

    internal static void Delete(IDbConnection dbClient, int roomId) => dbClient.Execute(
        "DELETE FROM `room` WHERE id = '" + roomId + "'");

    internal static void UpdateAll(IDbConnection dbClient, int roomId, string name, string description, string password, string tags, int categoryId, string state, int maxUsers, bool allowPets, bool allowPetsEat, bool allowWalkthrough, bool hideWall, int floorThickness, int wallThickness, int muteFuse, int kickFuse, int banFuse, int chatType, int chatBalloon, int chatSpeed, int chatMaxDistance, int chatFloodProtection, int trocStatus) => dbClient.Execute(
        @"UPDATE room SET caption = @Name, description = @Description, password = @Password, category = @CategoryId, state = @State, tags = @Tags, 
        users_max = @MaxUsers, allow_pets = @AllowPets, allow_pets_eat = @AllowPetsEat, allow_walkthrough = @AllowWalkthrough, allow_hidewall = @HideWall, 
        floorthick = @FloorThickness, wallthick = @WallThickness, moderation_mute_fuse = @MuteFuse, moderation_kick_fuse = @KickFuse, 
        moderation_ban_fuse = @BanFuse, chat_type = @ChatType, chat_balloon = @ChatBalloon, chat_speed = @ChatSpeed, chat_max_distance = @ChatMaxDistance,
        chat_flood_protection = @ChatFloodProtection, troc_status = @TrocStatus 
        WHERE id = @RoomId",
        new
        {
            RoomId = roomId,
            Name = name,
            Description = description,
            Password = password,
            Tags = tags,
            CategoryId = categoryId,
            State = state,
            MaxUsers = maxUsers,
            AllowPets = allowPets ? 1 : 0,
            AllowPetsEat = allowPetsEat ? 1 : 0,
            AllowWalkthrough = allowWalkthrough ? 1 : 0,
            HideWall = hideWall ? 1 : 0,
            FloorThickness = floorThickness,
            WallThickness = wallThickness,
            MuteFuse = muteFuse,
            KickFuse = kickFuse,
            BanFuse = banFuse,
            ChatType = chatType,
            ChatBalloon = chatBalloon,
            ChatSpeed = chatSpeed,
            ChatMaxDistance = chatMaxDistance,
            ChatFloodProtection = chatFloodProtection,
            TrocStatus = trocStatus
        });

    internal static void UpdateOwner(IDbConnection dbClient, string newUsername, string username) => dbClient.Execute(
        "UPDATE room SET owner = @NewUsername WHERE owner = @Username",
        new { NewUsername = newUsername, Username = username });

    internal static void UpdateOwnerByRoomId(IDbConnection dbClient, string newUsername, int roomId) => dbClient.Execute(
        "UPDATE room SET owner = @NewUsername WHERE id = @RoomId",
        new { NewUsername = newUsername, RoomId = roomId });

    internal static List<int> GetAllIdByOwner(IDbConnection dbClient, string username) => dbClient.Query<int>(
        "SELECT id FROM `room` WHERE owner = @Owner",
        new { Owner = username }
    ).ToList();

    internal static void UpdateResetUsersNow(IDbConnection dbClient) => dbClient.Execute(
        "UPDATE `room` SET users_now = '0' WHERE users_now > '0'");

    internal static int InsertDuplicate(IDbConnection dbClient, string username, string desc) => dbClient.ExecuteScalar<int>(
        @"INSERT INTO room (caption, description, owner, model_name, category, state, wallpaper, floor, landscape, allow_hidewall, wallthick, floorthick) 
        SELECT @Caption, @Desc, @Username, 'model_welcome', category, state, wallpaper, floor, landscape, allow_hidewall, wallthick, floorthick 
        FROM room WHERE id = '5328079'; 
        SELECT LAST_INSERT_ID();",
        new { Caption = username, Desc = desc, Username = username });

    internal static List<RoomEntity> GetAllSearchByUsername(IDbConnection dbClient, string searchData) => dbClient.Query<RoomEntity>(
        @"SELECT room.id, room.caption, room.owner, room.description, room.category, room.state, room.users_max, room.model_name, room.score, room.tags, 
        room.password, room.wallpaper, room.floor, room.landscape, room.allow_pets, room.allow_pets_eat, room.allow_walkthrough, room.allow_hidewall, 
        room.wallthick, room.floorthick, room.moderation_mute_fuse, room.allow_rightsoverride, room.moderation_kick_fuse, room.moderation_ban_fuse, 
        room.group_id, room.chat_type, room.chat_balloon, room.chat_speed, room.chat_max_distance, room.chat_flood_protection, room.troc_status, 
        room.users_now, room.allow_hidewireds, room.price, room.wired_security, user.id as owner_id, user.langue 
        FROM room 
        LEFT JOIN user ON room.owner = user.username 
        WHERE room.owner = @Username AND room.state != 'invisible' 
        ORDER BY room.users_now DESC",
        new { Username = searchData }
    ).ToList();

    internal static List<RoomEntity> GetAllSearch(IDbConnection dbClient, string searchData) => dbClient.Query<RoomEntity>(
        @"SELECT room.`id`, room.`caption`, room.`owner`, room.`description`, room.`category`, room.`state`, room.`users_max`, room.`model_name`, 
        room.`score`, room.`tags`, room.`password`, room.`wallpaper`, room.`floor`, room.`landscape`, room.`allow_pets`, room.`allow_pets_eat`, 
        room.`allow_walkthrough`, room.`allow_hidewall`, room.`wallthick`, room.`floorthick`, room.`moderation_mute_fuse`, room.`allow_rightsoverride`, 
        room.`moderation_kick_fuse`, room.`moderation_ban_fuse`, room.`group_id`, room.`chat_type`, room.`chat_balloon`, room.`chat_speed`, 
        room.`chat_max_distance`, room.`chat_flood_protection`, room.`troc_status`, room.`users_now`, room.`allow_hidewireds`, room.`price`, 
        room.`wired_security`, user.`id` as `owner_id`, user.`langue` 
        FROM `room` 
        LEFT JOIN user ON room.owner = user.username 
        WHERE room.caption LIKE @query OR room.owner LIKE @Query 
        ORDER BY room.users_now DESC 
        LIMIT 50",
        new { Query = searchData.Replace("%", "\\%").Replace("_", "\\_") + "%" }
    ).ToList();

    internal static int InsertDuplicate(IDbConnection dbClient, int roomId, string username) => dbClient.ExecuteScalar<int>(
        @"INSERT INTO room (caption, owner, description, model_name, wallpaper, floor, landscape, allow_hidewall, wallthick, floorthick, allow_rightsoverride, allow_hidewireds)
        SELECT CONCAT(caption, ' (Copie)'), @Username, description, model_name, wallpaper, floor, landscape, allow_hidewall, wallthick, floorthick, allow_rightsoverride, allow_hidewireds 
        FROM room 
        WHERE id = @RoomId;
        SELECT LAST_INSERT_ID();",
        new { RoomId = roomId, Username = username });

    internal static void UpdateModel(IDbConnection dbClient, int roomId) => dbClient.Execute(
        "UPDATE `room` SET model_name = 'model_custom' WHERE id = '" + roomId + "' LIMIT 1");

    internal static void UpdateHideWireds(IDbConnection dbClient, int roomId, bool hideWireds) => dbClient.Execute(
        "UPDATE `room` SET allow_hidewireds = '" + (hideWireds ? 1 : 0) + "' WHERE id = '" + roomId + "'");

    internal static void UpdateOwner(IDbConnection dbClient, int roomId, string username) => dbClient.Execute(
        "UPDATE room SET owner = @NewOwner WHERE id = @RoomId",
        new { NewOwner = username, RoomId = roomId });

    internal static void UpdatePrice(IDbConnection dbClient, int roomId, int price) => dbClient.Execute(
        "UPDATE room SET price = @Price WHERE id = @RoomId LIMIT 1",
        new { RoomId = roomId, Price = price });

    internal static void UpdateUsersMax(IDbConnection dbClient, int roomId, int maxUsers) => dbClient.Execute(
        "UPDATE `room` SET users_max = '" + maxUsers + "' WHERE id = '" + roomId + "'");

    internal static RoomEntity GetOne(IDbConnection dbClient, int roomId) => dbClient.QuerySingleOrDefault<RoomEntity>(
        @"SELECT room.`id`, room.`caption`, room.`owner`, room.`description`, room.`category`, room.`state`, room.`users_max`, room.`model_name`, room.`score`, room.`tags`, room.`password`, room.`wallpaper`, room.`floor`, room.`landscape`, room.`allow_pets`, room.`allow_pets_eat`, room.`allow_walkthrough`, room.`allow_hidewall`, room.`wallthick`, room.`floorthick`, room.`moderation_mute_fuse`, room.`allow_rightsoverride`, room.`moderation_kick_fuse`, room.`moderation_ban_fuse`, room.`group_id`, room.`chat_type`, room.`chat_balloon`, room.`chat_speed`, room.`chat_max_distance`, room.`chat_flood_protection`, room.`troc_status`, room.`users_now`, room.`allow_hidewireds`, room.`price`, room.`wired_security`, user.`id` as `owner_id`, user.`langue` 
        FROM `room` 
        LEFT JOIN user ON room.owner = user.username 
        WHERE room.id = @RoomId",
        new { RoomId = roomId });

    internal static int Insert(IDbConnection dbClient, string name, string desc, string username, string model, int category, int maxVisitors, int tradeSettings) => dbClient.ExecuteScalar<int>(
        @"INSERT INTO room (caption, description, owner, model_name, category, users_max, troc_status) 
        VALUES (@Name, @Desc, @Username, @Model, @Category, @MaxVisitors, @TradeSettings); 
        SELECT LAST_INSERT_ID();",
        new { Name = name, Desc = desc, Username = username, Model = model, Category = category, MaxVisitors = maxVisitors, TradeSettings = tradeSettings });

    internal static void UpdateUsersNow(IDbConnection dbClient, int roomId, int count) => dbClient.Execute(
        "UPDATE `room` SET users_now = '" + count + "' WHERE id = '" + roomId + "'");

    internal static void UpdateState(IDbConnection dbClient, int roomId) => dbClient.Execute(
        "UPDATE `room` SET state = 'locked' WHERE id = '" + roomId + "'");

    internal static void UpdateCaptionDescTags(IDbConnection dbClient, int roomId) => dbClient.Execute(
        "UPDATE `room` SET caption = 'Cet appart ne respect par les conditions dutilisation', description = 'Cet appart ne respect par les conditions dutilisation', tags = '' WHERE id = '" + roomId + "'");
}

public class RoomEntity
{
    public int Id { get; set; }
    public string Caption { get; set; }
    public string Owner { get; set; }
    public int OwnerId { get; set; }
    public string Langue { get; set; }
    public string Description { get; set; }
    public int Category { get; set; }
    public RoomState State { get; set; }
    public int UsersMax { get; set; }
    public string ModelName { get; set; }
    public int Score { get; set; }
    public string Tags { get; set; }
    public string Password { get; set; }
    public string Wallpaper { get; set; }
    public string Floor { get; set; }
    public string Landscape { get; set; }
    public bool AllowPets { get; set; }
    public bool AllowPetsEat { get; set; }
    public bool AllowWalkthrough { get; set; }
    public bool AllowHideWall { get; set; }
    public int WallThick { get; set; }
    public int FloorThick { get; set; }
    public int ModerationMuteFuse { get; set; }
    public bool AllowRightsOverride { get; set; }
    public int ModerationKickFuse { get; set; }
    public int ModerationBanFuse { get; set; }
    public int GroupId { get; set; }
    public int ChatType { get; set; }
    public int ChatBalloon { get; set; }
    public int ChatSpeed { get; set; }
    public int ChatMaxDistance { get; set; }
    public int ChatFloodProtection { get; set; }
    public int TrocStatus { get; set; }
    public int UsersNow { get; set; }
    public bool AllowHideWireds { get; set; }
    public int Price { get; set; }
    public bool WiredSecurity { get; set; }
}

public enum RoomState
{
    Open,
    Locked,
    Password,
    Hide
}
