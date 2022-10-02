using System.Data;
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
