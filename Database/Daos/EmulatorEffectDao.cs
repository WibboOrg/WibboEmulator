using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class EmulatorEffectDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, only_staff FROM systeme_effects ORDER by id ASC");
            return dbClient.GetTable();
        }
    }
}