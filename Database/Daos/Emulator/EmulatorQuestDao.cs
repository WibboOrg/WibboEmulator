namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal sealed class EmulatorQuestDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT `id`, `category`, `series_number`, `goal_type`, `goal_data`, `name`, `reward`, `data_bit` FROM `emulator_quest`");

        return dbClient.GetTable();
    }
}