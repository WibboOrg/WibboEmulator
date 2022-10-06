namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal class EmulatorCommandDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT `id`, `input`, `minrank`, `description_fr`, `description_en`, `description_br` FROM `emulator_command`");
        return dbClient.GetTable();
    }
}