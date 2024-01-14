namespace WibboEmulator.Database.Daos.Item;
using System.Data;
using Dapper;

internal sealed class ItemStatDao
{
    internal static int GetOne(IDbConnection dbClient, int baseId) => dbClient.QueryFirstOrDefault<int>(
        "SELECT amount FROM item_stat WHERE base_id = @BaseId LIMIT 1",
        new { BaseId = baseId });

    internal static void UpdateRemove(IDbConnection dbClient, Dictionary<int, int> rareAmounts)
    {
        if (rareAmounts.Count == 0)
        {
            return;
        }

        var itemStats = new List<ItemStatEntity>();

        foreach (var rare in rareAmounts)
        {
            itemStats.Add(new ItemStatEntity { Amount = rare.Value, BaseId = rare.Key });
        }

        _ = dbClient.Execute(
            "UPDATE item_stat SET amount = amount - @Amount WHERE base_id = @BaseId",
            itemStats);
    }

    internal static void UpdateAdd(IDbConnection dbClient, int baseId, int amount = 1) => dbClient.Execute(
        "UPDATE `item_stat` SET amount = amount + '" + amount + "' WHERE base_id = '" + baseId + "'");
}

public class ItemStatEntity
{
    public int Amount { get; set; }
    public int BaseId { get; set; }
}