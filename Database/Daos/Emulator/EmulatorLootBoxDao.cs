using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WibboEmulator.Database.Interfaces;

namespace WibboEmulator.Database.Daos.Emulator
{
    public class EmulatorLootBoxDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT `interaction_type`, `probability`, `page_id`, `item_id`, `category`, `amount` FROM `emulator_lootbox`");
            return dbClient.GetTable();
        }
    }
}
