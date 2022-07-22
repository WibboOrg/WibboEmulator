using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Items;

namespace WibboEmulator.Game.Loots
{
    public class LootManager
    {
        private readonly Dictionary<InteractionType, List<Loot>> LootItem;

        public LootManager()
        {
            this.LootItem = new Dictionary<InteractionType, List<Loot>>();
        }

        public void Init(IQueryAdapter dbClient)
        {
            this.LootItem.Clear();

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

        public List<Loot> GetLoots(InteractionType interactionType)
        {
            this.LootItem.TryGetValue(interactionType, out List<Loot> loots);

            return loots;
        }
    }
}
