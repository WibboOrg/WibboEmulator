namespace WibboEmulator.Database.Daos.Room;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal sealed class RoomSwearwordFilterDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT word FROM `room_swearword_filter`");
        return dbClient.GetTable();
    }
}