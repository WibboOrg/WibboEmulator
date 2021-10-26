using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class EmulatorEffectDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, only_staff FROM systeme_effects ORDER by id ASC");
            DataTable table = dbClient.GetTable();
        }
    }
}