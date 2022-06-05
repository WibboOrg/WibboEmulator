using Wibbo.Database.Interfaces;
using System.Data;

namespace Wibbo.Database.Daos
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
