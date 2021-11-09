using System.Data;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class EmulatorTextDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT identifiant, value_fr, value_en, value_br FROM system_locale");
            return dbClient.GetTable();
        }
    }
}