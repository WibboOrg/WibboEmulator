namespace WibboEmulator.Database.Daos;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal class EmulatorCommandPetDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT `id`, `command` FROM `emulator_command_pet`");
        return dbClient.GetTable();
    }
}