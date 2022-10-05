using System.Data;
using WibboEmulator.Database.Interfaces;

namespace WibboEmulator.Database.Daos
{
    class CatalogBotPresetDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT `id`, `name`, `figure`, `motto`, `gender`, `ai_type` FROM `catalog_bot_preset`");
            return dbClient.GetTable();
        }
    }
}
