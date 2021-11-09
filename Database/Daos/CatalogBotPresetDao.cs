using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class CatalogBotPresetDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, name, figure, motto, gender, ai_type FROM catalog_bot_presets");
            return dbClient.GetTable();
        }
    }
}
