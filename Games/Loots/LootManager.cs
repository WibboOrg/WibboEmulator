namespace WibboEmulator.Games.Loots;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items;

public class LootManager
{
    private readonly Dictionary<InteractionType, List<Loot>> _lootItem;
    private readonly Dictionary<int, int> _rarityCounter;

    public LootManager()
    {
        this._lootItem = new Dictionary<InteractionType, List<Loot>>();
        this._rarityCounter = new Dictionary<int, int>();
    }

    public void Init(IQueryAdapter dbClient)
    {
        this._lootItem.Clear();

        var timeNow = DateTime.Now;
        var startMounth = new DateTime(timeNow.Year, timeNow.Month, 1);

        var timestampStartMounth = (int)startMounth.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        var basicCounter = LogLootBoxDao.GetCount(dbClient, "LOOTBOX_2022", timestampStartMounth, 1);
        var commmCounter = LogLootBoxDao.GetCount(dbClient, "LOOTBOX_2022", timestampStartMounth, 2);
        var epicCounter = LogLootBoxDao.GetCount(dbClient, "LOOTBOX_2022", timestampStartMounth, 3);
        var legendaryCounter = LogLootBoxDao.GetCount(dbClient, "LOOTBOX_2022", timestampStartMounth, 4);

        this._rarityCounter.Add(1, basicCounter);
        this._rarityCounter.Add(2, commmCounter);
        this._rarityCounter.Add(3, epicCounter);
        this._rarityCounter.Add(4, legendaryCounter);

        var dTable = EmulatorLootBoxDao.GetAll(dbClient);

        foreach (DataRow dRow in dTable.Rows)
        {
            var interactionType = InteractionTypes.GetTypeFromString(dRow["interaction_type"].ToString());
            var loot = new Loot(Convert.ToInt32(dRow["probability"].ToString()), Convert.ToInt32(dRow["page_id"].ToString()), Convert.ToInt32(dRow["item_id"].ToString()), dRow["category"].ToString(), Convert.ToInt32(dRow["amount"].ToString()));

            if (!this._lootItem.ContainsKey(interactionType))
            {
                this._lootItem.Add(interactionType, new List<Loot>());
            }

            if (this._lootItem.TryGetValue(interactionType, out var loots))
            {
                loots.Add(loot);
            }
        }
    }

    public int GetRarityCounter(int rarityLevel)
    {
        _ = this._rarityCounter.TryGetValue(rarityLevel, out var count);

        return count;
    }

    public void IncrementeRarityCounter(int rarityLevel) => this._rarityCounter[rarityLevel] += 1;

    public List<Loot> GetLoots(InteractionType interactionType)
    {
        _ = this._lootItem.TryGetValue(interactionType, out var loots);

        return loots;
    }
}
