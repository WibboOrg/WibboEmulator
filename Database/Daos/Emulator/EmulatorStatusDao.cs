namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using Dapper;

internal sealed class EmulatorStatusDao
{
    internal static int GetUserpeak(IDbConnection dbClient) => dbClient.ExecuteScalar<int>(
        "SELECT userpeak FROM emulator_status");

    internal static void UpdateScore(IDbConnection dbClient, int usersOnline, int roomsLoaded, int userPeak) => dbClient.Execute(
        "UPDATE `emulator_status` SET users_online = '" + usersOnline + "', rooms_loaded = '" + roomsLoaded + "', userpeak = '" + userPeak + "', stamp = UNIX_TIMESTAMP()");

    internal static void UpdateReset(IDbConnection dbClient) => dbClient.Execute(
        "UPDATE `emulator_status` SET users_online = '0', rooms_loaded = '0', stamp = '" + WibboEnvironment.GetUnixTimestamp() + "'");
}