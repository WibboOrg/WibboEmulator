using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WibboEmulator.Games.Loots
{
    public class Loot
    {
        public int Probability { get; private set; }
        public int PageId { get; private set; }
        public int ItemId { get; private set; }
        public string Category { get; private set; }
        public int Amount { get; private set; }

        public Loot(int probability, int pageId, int itemId, string category, int amount)
        {
            this.Probability = probability;
            this.PageId = pageId;
            this.ItemId = itemId;
            this.Category = category;
            this.Amount = amount;
        }
    }
}
