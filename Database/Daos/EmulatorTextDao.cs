using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class EmulatorTextDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT identifiant, value_fr, value_en, value_br FROM system_locale");
            table = dbClient.GetTable();
        }
    }
}