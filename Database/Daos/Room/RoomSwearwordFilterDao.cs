namespace WibboEmulator.Database.Daos;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal class RoomSwearwordFilterDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT word FROM `room_swearword_filter`");
        return dbClient.GetTable();
    }
}