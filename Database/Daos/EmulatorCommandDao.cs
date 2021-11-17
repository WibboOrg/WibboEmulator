using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class EmulatorCommandDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM `emulator_command`");
            return dbClient.GetTable();
        }
    }
}