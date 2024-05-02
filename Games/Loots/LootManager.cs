namespace WibboEmulator.Games.Loots;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Games.Items;

public static class LootManager
{
    private static readonly Dictionary<InteractionType, List<Loot>> LootItem = [];
    private static readonly Dictionary<int, int> RarityCounter = [];

    public static void Initialize(IDbConnection dbClient)
    {
        LootItem.Clear();
        RarityCounter.Clear();

        var timeNow = DateTime.Now;
        var startMounth = new DateTime(timeNow.Year, timeNow.Month, 1);

        var timestampStartMounth = (int)startMounth.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        var basicCounter = LogLootBoxDao.GetCount(dbClient, "LOOTBOX_2022", timestampStartMounth, 1);
        var commmCounter = LogLootBoxDao.GetCount(dbClient, "LOOTBOX_2022", timestampStartMounth, 2);
        var epicCounter = LogLootBoxDao.GetCount(dbClient, "LOOTBOX_2022", timestampStartMounth, 3);
        var legendaryCounter = LogLootBoxDao.GetCount(dbClient, "LOOTBOX_2022", timestampStartMounth, 4);

        RarityCounter.Add(1, basicCounter);
        RarityCounter.Add(2, commmCounter);
        RarityCounter.Add(3, epicCounter);
        RarityCounter.Add(4, legendaryCounter);

        var emulatorLootBoxList = EmulatorLootBoxDao.GetAll(dbClient);

        foreach (var emulatorLootBox in emulatorLootBoxList)
        {
            var interactionType = InteractionTypes.GetTypeFromString(emulatorLootBox.InteractionType);
            var loot = new Loot(emulatorLootBox.Probability, emulatorLootBox.PageId, emulatorLootBox.ItemId, emulatorLootBox.Category.ToString(), emulatorLootBox.Amount);

            if (!LootItem.ContainsKey(interactionType))
            {
                LootItem.Add(interactionType, []);
            }

            if (LootItem.TryGetValue(interactionType, out var loots))
            {
                loots.Add(loot);
            }
        }
    }

    public static int GetRarityCounter(int rarityLevel)
    {
        _ = RarityCounter.TryGetValue(rarityLevel, out var count);

        return count;
    }

    public static void IncrementeRarityCounter(int rarityLevel) => RarityCounter[rarityLevel] += 1;

    public static List<Loot> GetLoots(InteractionType interactionType)
    {
        _ = LootItem.TryGetValue(interactionType, out var loots);

        return loots;
    }
}
