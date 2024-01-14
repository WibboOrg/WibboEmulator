namespace WibboEmulator.Database.Daos.Room;
using System.Data;
using Dapper;

internal sealed class RoomModelDao
{
    internal static List<RoomModelEntity> GetAll(IDbConnection dbClient) => dbClient.Query<RoomModelEntity>(
        "SELECT id, door_x, door_y, door_z, door_dir, heightmap FROM `room_model`"
    ).ToList();
}

public class RoomModelEntity
{
    public string Id { get; set; }
    public int DoorX { get; set; }
    public int DoorY { get; set; }
    public double DoorZ { get; set; }
    public int DoorDir { get; set; }
    public string Heightmap { get; set; }
}