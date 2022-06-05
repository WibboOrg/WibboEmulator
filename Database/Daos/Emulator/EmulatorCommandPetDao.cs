using Wibbo.Database.Interfaces;
using System.Data;

namespace Wibbo.Database.Daos
{
    class EmulatorCommandPetDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT `id`, `command` FROM `emulator_command_pet`");
            return dbClient.GetTable();
        }
    }
}