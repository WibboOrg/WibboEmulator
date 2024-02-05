namespace WibboEmulator.Database.Daos.Item;
using System.Collections.Concurrent;
using System.Data;
using Dapper;
using WibboEmulator.Games.Items;

internal sealed class ItemDao
{
    internal static void SaveUpdateItems(IDbConnection dbClient, ConcurrentDictionary<int, Item> updateItems)
    {
        if (updateItems.IsEmpty)
        {
            return;
        }

        var updateFloor = new List<ItemEntity>();
        var updateWall = new List<ItemEntity>();

        foreach (var roomItem in updateItems.Values)
        {
            if (roomItem.IsWallItem)
            {
                updateWall.Add(new ItemEntity { WallPos = roomItem.WallCoord, Id = roomItem.Id });
            }
            else
            {
                updateFloor.Add(new ItemEntity { X = roomItem.X, Y = roomItem.Y, Z = roomItem.Z, Rot = roomItem.Rotation, ExtraData = roomItem.ExtraData, Id = roomItem.Id });
            }
        }

        if (updateWall.Count != 0)
        {
            _ = dbClient.Execute(
                @"UPDATE item 
                SET wall_pos = @WallPos 
                WHERE id = @Id",
                updateWall);
        }

        if (updateFloor.Count != 0)
        {
            _ = dbClient.Execute(
                @"UPDATE item 
                SET x = @X, y = @Y, z = @Z, rot = @Rot, extra_data = @ExtraData 
                WHERE id = @Id",
                updateFloor);
        }
    }

    internal static void UpdateItems(IDbConnection dbClient, List<Item> updateItems, int userId)
    {
        if (updateItems.Count == 0)
        {
            return;
        }

        var itemIds = updateItems.Select(item => item.Id).ToList();

        _ = dbClient.Execute(
            @"UPDATE item 
            SET room_id = '0', user_id = @UserId 
            WHERE id IN @ItemIds",
            new { UserId = userId, ItemIds = itemIds });
    }

    internal static void DeleteItems(IDbConnection dbClient, List<Item> deleteItems)
    {
        if (deleteItems.Count == 0)
        {
            return;
        }

        var itemIds = deleteItems.Select(item => item.Id).ToList();

        _ = dbClient.Execute(
            @"DELETE item, item_limited, item_present, item_moodlight, item_teleport, item_wired 
            FROM item 
            LEFT JOIN item_limited ON (item_limited.item_id = item.id) 
            LEFT JOIN item_present ON (item_present.item_id = item.id) 
            LEFT JOIN item_moodlight ON (item_moodlight.item_id = item.id) 
            LEFT JOIN item_teleport ON (tele_one_id = item.id) 
            LEFT JOIN item_wired ON (trigger_id = item.id) 
            WHERE id IN @ItemIds",
            new { ItemIds = itemIds });
    }

    internal static int Insert(IDbConnection dbClient, int baseItem, int userId, string extraData) => dbClient.ExecuteScalar<int>(
        @"INSERT INTO item (base_item, user_id, extra_data) 
        VALUES (@BaseItem, @UserId, @ExtraData);
        SELECT LAST_INSERT_ID();",
        new { BaseItem = baseItem, UserId = userId, ExtraData = extraData });

    internal static void Insert(IDbConnection dbClient, int itemId, int baseItem, int userId, string extraData) => dbClient.Execute(
        @"INSERT INTO item (id, base_item, user_id, extra_data) 
        VALUES (@ItemId, @BaseItem, @UserId, @ExtraData);",
        new { ItemId = itemId, BaseItem = baseItem, UserId = userId, ExtraData = extraData });

    internal static int InsertDuplicate(IDbConnection dbClient, int userId, int roomId, int itemId) => dbClient.ExecuteScalar<int>(
        @"INSERT INTO item (user_id, room_id, base_item, extra_data, x, y, z, rot, wall_pos)
        SELECT @UserId, @RoomId, base_item, extra_data, x, y, z, rot, wall_pos 
        FROM item 
        WHERE id = @ItemId;
        SELECT LAST_INSERT_ID();",
        new { UserId = userId, RoomId = roomId, ItemId = itemId });

    internal static void InsertDuplicate(IDbConnection dbClient, int userId, int roomId) => dbClient.Execute(
        "INSERT INTO `item` (user_id, room_id, base_item, extra_data, x, y, z, rot) SELECT '" + userId + "', '" + roomId + "', base_item, extra_data, x, y, z, rot FROM `item` WHERE room_id = '5328079'");

    internal static void Delete(IDbConnection dbClient, int itemId) => dbClient.Execute(
        "DELETE `item`, `item_limited` FROM `item` LEFT JOIN `item_limited` ON(`item_limited`.item_id = `item`.id) WHERE id = '" + itemId + "'");

    internal static void DeleteById(IDbConnection dbClient, int itemId) => dbClient.Execute(
        "DELETE `item`, `item_limited`, `item_present`, `item_moodlight`, `item_teleport`, `item_wired` FROM `item` LEFT JOIN `item_limited` ON (`item_limited`.item_id = `item`.id) LEFT JOIN `item_present` ON (`item_present`.item_id = `item`.id) LEFT JOIN `item_moodlight` ON (`item_moodlight`.item_id = `item`.id) LEFT JOIN `item_teleport` ON (tele_one_id = `item`.id) LEFT JOIN `item_wired` ON (trigger_id = `item`.id) WHERE id = '" + itemId + "'");

    internal static void DeleteAllByRoomId(IDbConnection dbClient, int roomId) => dbClient.Execute(
        "DELETE `item`, `item_limited`, `item_present`, `item_moodlight`, `item_teleport`, `item_wired` FROM `item` LEFT JOIN `item_limited` ON (`item_limited`.item_id = `item`.id) LEFT JOIN `item_present` ON (`item_present`.item_id = `item`.id) LEFT JOIN `item_moodlight` ON (`item_moodlight`.item_id = `item`.id) LEFT JOIN `item_teleport` ON (tele_one_id = `item`.id) LEFT JOIN `item_wired` ON (trigger_id = `item`.id) WHERE room_id = '" + roomId + "'");

    internal static void DeleteAllByRoomIdAndBaseItem(IDbConnection dbClient, int roomId, int userId, int baseItem) => dbClient.Execute(
        "DELETE `item`, `item_limited`, `item_present`, `item_moodlight`, `item_teleport`, `item_wired` FROM `item` LEFT JOIN `item_limited` ON (`item_limited`.item_id = `item`.id) LEFT JOIN `item_present` ON (`item_present`.item_id = `item`.id) LEFT JOIN `item_moodlight` ON (`item_moodlight`.item_id = `item`.id) LEFT JOIN `item_teleport` ON (tele_one_id = `item`.id) LEFT JOIN `item_wired` ON (trigger_id = `item`.id) WHERE user_id = '" + userId + "' AND room_id = '" + roomId + "' AND base_item = '" + baseItem + "'");

    internal static void DeleteAll(IDbConnection dbClient, int userId) => dbClient.Execute(
        "DELETE `item`, `item_limited`, `item_present`, `item_moodlight`, `item_teleport`, `item_wired` FROM `item` LEFT JOIN `item_limited` ON (`item_limited`.item_id = `item`.id) LEFT JOIN `item_present` ON (`item_present`.item_id = `item`.id) LEFT JOIN `item_moodlight` ON (`item_moodlight`.item_id = `item`.id) LEFT JOIN `item_teleport` ON (tele_one_id = `item`.id) LEFT JOIN `item_wired` ON (trigger_id = `item`.id) WHERE room_id = '0' AND user_id = '" + userId + "'");

    internal static void DeleteAllWithoutRare(IDbConnection dbClient, int userId) => dbClient.Execute(
        "DELETE `item`, `item_limited`, `item_present`, `item_moodlight`, `item_teleport`, `item_wired` FROM `item` LEFT JOIN `item_limited` ON (`item_limited`.item_id = `item`.id) LEFT JOIN `item_present` ON (`item_present`.item_id = `item`.id) LEFT JOIN `item_moodlight` ON (`item_moodlight`.item_id = `item`.id) LEFT JOIN `item_teleport` ON (tele_one_id = `item`.id) LEFT JOIN `item_wired` ON (trigger_id = `item`.id) WHERE room_id = '0' AND user_id = '" + userId + "' AND base_item NOT IN (SELECT id FROM `item_base` WHERE is_rare = '1' OR interaction_type = 'trophy' OR interaction_type = 'gift')");

    internal static void UpdateExtradata(IDbConnection dbClient, int itemId, string extraData) => dbClient.Execute(
        "UPDATE item SET extra_data = @ExtraData WHERE id = @ItemId LIMIT 1",
        new { ExtraData = extraData, ItemId = itemId });

    internal static void UpdateBaseItem(IDbConnection dbClient, int itemId, int baseItem) => dbClient.Execute(
        "UPDATE item SET base_item = @BaseItem WHERE id = @ItemId",
        new { BaseItem = baseItem, ItemId = itemId });

    internal static void UpdateBaseItemAndExtraData(IDbConnection dbClient, int itemId, int baseItem, string extraData) => dbClient.Execute(
        "UPDATE item SET base_item = @BaseItem, extra_data = @ExtraData WHERE id = @ItemId",
        new { BaseItem = baseItem, ExtraData = extraData, ItemId = itemId });

    internal static void UpdateRoomIdAndUserId(IDbConnection dbClient, int itemId, int roomId, int userId) => dbClient.Execute(
        "UPDATE `item` SET room_id = '" + roomId + "', user_id = '" + userId + "' WHERE id = '" + itemId + "'");

    internal static void UpdateRoomIdAndUserId(IDbConnection dbClient, int userId, int roomId) => dbClient.Execute(
        "UPDATE `item` SET room_id = '0', user_id = '" + userId + "' WHERE room_id = '" + roomId + "'");

    internal static void UpdateResetRoomId(IDbConnection dbClient, int itemId) => dbClient.Execute(
        "UPDATE item SET room_id = '0' WHERE id = @ItemId LIMIT 1",
        new { ItemId = itemId });

    internal static int GetOneRoomId(IDbConnection dbClient, int itemId) => dbClient.QuerySingleOrDefault<int>(
        "SELECT room_id FROM `item` WHERE id = '" + itemId + "' LIMIT 1");

    internal static List<ItemEntity> GetAll(IDbConnection dbClient, int roomId) => dbClient.Query<ItemEntity>(
        @"SELECT `item`.id, `item`.user_id, `item`.room_id, `item`.base_item, `item`.extra_data, `item`.x, `item`.y, `item`.z, `item`.rot, `item`.wall_pos, `item_limited`.limited_number, `item_limited`.limited_stack, `item_wired`.trigger_data, `item_wired`.trigger_data_2, `item_wired`.triggers_item, `item_wired`.all_user_triggerable, `item_wired`.delay, `item_moodlight`.enabled, `item_moodlight`.current_preset, `item_moodlight`.preset_one, `item_moodlight`.preset_two, `item_moodlight`.preset_three 
        FROM `item` 
        LEFT JOIN `item_moodlight` ON (`item_moodlight`.item_id = `item`.id) 
        LEFT JOIN `item_wired` ON (trigger_id = `item`.id) 
        LEFT JOIN `item_limited` ON (`item_limited`.item_id = `item`.id) 
        WHERE `item`.room_id = @RoomId",
        new { RoomId = roomId }
    ).ToList();

    internal static int GetOneLimitedId(IDbConnection dbClient, int limitedNumber, int itemId) => dbClient.ExecuteScalar<int>(
        "SELECT id FROM `item` WHERE id IN (SELECT item_id FROM `item_limited` WHERE limited_number = '" + limitedNumber + "') AND base_item = '" + itemId + "' LIMIT 1");

    internal static List<ItemEntity> GetAllByUserId(IDbConnection dbClient, int userId, int limit) => dbClient.Query<ItemEntity>(
        "SELECT `item`.id, `item`.base_item, `item`.extra_data, `item_limited`.limited_number, `item_limited`.limited_stack FROM `item` LEFT JOIN `item_limited` ON (`item_limited`.item_id = `item`.id) WHERE `item`.user_id = @UserId AND `item`.room_id = '0' LIMIT @Limit",
        new { UserId = userId, Limit = limit }
    ).ToList();
}

public class ItemEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public int BaseItem { get; set; }
    public string ExtraData { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public double Z { get; set; }
    public int Rot { get; set; }
    public string WallPos { get; set; }

    public int? LimitedNumber { get; set; }
    public int? LimitedStack { get; set; }

    public string TriggerData2 { get; set; }
    public string TriggerData { get; set; }
    public bool AllUserTriggerable { get; set; }
    public string TriggersItem { get; set; }
    public int Delay { get; set; }

    public bool Enabled { get; set; }
    public int CurrentPreset { get; set; }
    public string PresetOne { get; set; }
    public string PresetTwo { get; set; }
    public string PresetThree { get; set; }
}
