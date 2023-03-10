namespace WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Utilities;

internal sealed class ItemStatDao
{
    internal static int GetOne(IQueryAdapter dbClient, int baseId)
    {
        dbClient.SetQuery("SELECT amount FROM `item_stat` WHERE base_id = '" + baseId + "' LIMIT 1");

        return dbClient.GetInteger();
    }

    internal static void UpdateRemove(IQueryAdapter dbClient, Dictionary<int, int> rareAmounts)
    {
        if (rareAmounts.Count == 0)
        {
            return;
        }

        var standardQueries = new QueryChunk();

        foreach (var rare in rareAmounts)
        {
            standardQueries.AddQuery("UPDATE `item_stat` SET amount = amount - '" + rare.Value + "' WHERE base_id = '" + rare.Key + "'");
        }

        standardQueries.Execute(dbClient);
        standardQueries.Dispose();
    }

    internal static void UpdateAdd(IQueryAdapter dbClient, int baseId, int amount = 1) => dbClient.RunQuery("UPDATE `item_stat` SET amount = amount + '" + amount + "' WHERE base_id = '" + baseId + "'");
}
