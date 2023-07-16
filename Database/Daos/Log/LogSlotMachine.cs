namespace WibboEmulator.Database.Daos.Log;
using WibboEmulator.Database.Interfaces;

internal sealed class LogSlotMachineDao
{
    internal static void Insert(IQueryAdapter dbClient, int userId, int amount, bool isWin)
    {
        dbClient.SetQuery("INSERT INTO `log_slotmachine` (`user_id`, `amount`, `is_win`, `date`) VALUES ('" + userId + "', '" + amount + "', '" + (isWin ? "1" : "0") + "', UNIX_TIMESTAMP())");
        dbClient.RunQuery();
    }
}