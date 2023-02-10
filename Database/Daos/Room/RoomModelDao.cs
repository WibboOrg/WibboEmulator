namespace WibboEmulator.Database.Daos.Room;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal sealed class RoomModelDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT id, door_x, door_y, door_z, door_dir, heightmap FROM `room_model`");
        return dbClient.GetTable();
    }
}