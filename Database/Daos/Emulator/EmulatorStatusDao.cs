namespace WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Interfaces;

internal sealed class EmulatorStatusDao
{
    internal static int GetUserpeak(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT `userpeak` FROM `emulator_status`");
        return dbClient.GetInteger();
    }

    internal static void UpdateScore(IQueryAdapter dbClient, int usersOnline, int roomsLoaded, int userPeak) => dbClient.RunQuery("UPDATE `emulator_status` SET users_online = '" + usersOnline + "', rooms_loaded = '" + roomsLoaded + "', userpeak = '" + userPeak + "', stamp = UNIX_TIMESTAMP()");

    internal static void UpdateReset(IQueryAdapter dbClient) => dbClient.RunQuery("UPDATE `emulator_status` SET status = '1', users_online = '0', rooms_loaded = '0', stamp = '" + WibboEnvironment.GetUnixTimestamp() + "'");
}