namespace WibboEmulator.Database.Daos.User;

using System.Data;
using Dapper;

internal sealed class ItemPresentDao
{
    internal static void Insert(IDbConnection dbClient, int itemId, int baseId, string extraData) => dbClient.Execute(
        "INSERT INTO item_present (item_id, base_id, extra_data) VALUES (@ItemId, @BaseId, @ExtraData)",
        new { ItemId = itemId, BaseId = baseId, ExtraData = extraData });

    internal static ItemPresentEntity GetOne(IDbConnection dbClient, int itemId) => dbClient.QuerySingleOrDefault<ItemPresentEntity>(
        "SELECT `base_id`, `extra_data` FROM `item_present` WHERE `item_id` = @PresentId LIMIT 1",
        new { PresentId = itemId });

    internal static void Delete(IDbConnection dbClient, int itemId) => dbClient.Execute(
        "DELETE FROM `item_present` WHERE `item_id` = '" + itemId + "' LIMIT 1");
}

public class ItemPresentEntity
{
    public int ItemId { get; set; }
    public int BaseId { get; set; }
    public string ExtraData { get; set; }
}