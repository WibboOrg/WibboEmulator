namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using Dapper;

internal sealed class EmulatorStatsDao
{
    internal static void Insert(IDbConnection dbClient, int usersOnline, int roomsLoaded) => dbClient.Execute(
        "INSERT INTO `emulator_stats` (`online`, `time`, `room`) VALUES ('" + usersOnline + "', UNIX_TIMESTAMP(), '" + roomsLoaded + "')");
}