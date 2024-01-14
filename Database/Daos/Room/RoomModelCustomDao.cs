namespace WibboEmulator.Database.Daos.Room;
using System.Data;
using Dapper;

internal sealed class RoomModelCustomDao
{
    internal static void Replace(IDbConnection dbClient, int roomId, int doorX, int doorY, double doorZ, int doorDirection, string map, int wallHeight) => dbClient.Execute(
        "REPLACE INTO room_model_custom VALUES (@RoomId, @DoorX, @DoorY, @DoorZ, @DoorDirection, @Map, @WallHeight)",
        new { RoomId = roomId, DoorX = doorX, DoorY = doorY, DoorZ = doorZ.ToString(), DoorDirection = doorDirection, Map = map, WallHeight = wallHeight });

    internal static void InsertDuplicate(IDbConnection dbClient, int roomId, int oldRoomId) => dbClient.Execute(
        @"INSERT INTO `room_model_custom` (room_id, door_x, door_y, door_z, door_dir, heightmap, wall_height)
        SELECT '" + roomId + "', door_x, door_y, door_z, door_dir, heightmap, wall_height FROM `room_model_custom` WHERE room_id = '" + oldRoomId + "'");

    internal static RoomModelCustomEntity GetOne(IDbConnection dbClient, int roomId) => dbClient.QuerySingleOrDefault<RoomModelCustomEntity>(
        "SELECT door_x, door_y, door_z, door_dir, heightmap, wall_height FROM `room_model_custom` WHERE room_id = '" + roomId + "'");
}

public class RoomModelCustomEntity
{
    public int RoomId { get; set; }
    public int DoorX { get; set; }
    public int DoorY { get; set; }
    public double DoorZ { get; set; }
    public int DoorDir { get; set; }
    public string Heightmap { get; set; }
    public int WallHeight { get; set; }
}