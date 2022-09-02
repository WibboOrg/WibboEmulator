using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Items;

namespace WibboEmulator.Game.Loots
{
    public class LootManager
    {
        private readonly Dictionary<InteractionType, List<Loot>> LootItem;
        private readonly Dictionary<int, int> RarityCounter;

        public LootManager()
        {
            this.LootItem = new Dictionary<InteractionType, List<Loot>>();
            this.RarityCounter = new Dictionary<int, int>();
        }

        public void Init(IQueryAdapter dbClient)
        {
            this.LootItem.Clear();

            DateTime timeNow = DateTime.Now;
            DateTime startMounth = new DateTime(timeNow.Year, timeNow.Month, 1);

            int timestampStartMounth = (int)startMounth.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            int commmCounter = LogLootBoxDao.GetCount(dbClient, "LOOTBOX2022", timestampStartMounth, 2);
            int epicCounter = LogLootBoxDao.GetCount(dbClient, "LOOTBOX2022", timestampStartMounth, 3);
            int legendaryCounter = LogLootBoxDao.GetCount(dbClient, "LOOTBOX2022", timestampStartMounth, 4);

            this.RarityCounter.Add(2, commmCounter);
            this.RarityCounter.Add(3, epicCounter);
            this.RarityCounter.Add(4, legendaryCounter);

            DataTable dTable = EmulatorLootBoxDao.GetAll(dbClient);

            foreach (DataRow dRow in dTable.Rows)
            {
                InteractionType interactionType = InteractionTypes.GetTypeFromString(dRow["interaction_type"].ToString());
                Loot loot = new Loot(int.Parse(dRow["probability"].ToString()), int.Parse(dRow["page_id"].ToString()), int.Parse(dRow["item_id"].ToString()), dRow["category"].ToString(), int.Parse(dRow["amount"].ToString()));

                if (!this.LootItem.ContainsKey(interactionType))
                {
                    this.LootItem.Add(interactionType, new List<Loot>());
                }

                if (this.LootItem.TryGetValue(interactionType, out List<Loot> loots))
                {
                    loots.Add(loot);
                }
            }
        }

        public int GetRarityCounter(int rarityLevel)
        {
            this.RarityCounter.TryGetValue(rarityLevel, out int count);

            return count;
        }

        public void IncrementeRarityCounter(int rarityLevel)
        {
            this.RarityCounter[rarityLevel] += 1;
        }

        public List<Loot> GetLoots(InteractionType interactionType)
        {
            this.LootItem.TryGetValue(interactionType, out List<Loot> loots);

            return loots;
        }
    }
}
