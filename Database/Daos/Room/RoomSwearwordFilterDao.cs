namespace WibboEmulator.Database.Daos.Room;
using System.Data;
using Dapper;

internal sealed class RoomSwearwordFilterDao
{
    internal static List<string> GetAll(IDbConnection dbClient) => dbClient.Query<string>(
        "SELECT word FROM `room_swearword_filter`"
    ).ToList();
}