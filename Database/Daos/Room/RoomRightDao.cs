namespace WibboEmulator.Database.Daos.Room;
using System.Data;
using Dapper;

internal sealed class RoomRightDao
{
    internal static void Insert(IDbConnection dbClient, int roomId, int userId) => dbClient.Execute(
        "INSERT INTO `room_right` (room_id, user_id) VALUES ('" + roomId + "', '" + userId + "')");

    internal static void Delete(IDbConnection dbClient, int roomId) => dbClient.Execute(
        "DELETE FROM `room_right` WHERE room_id = '" + roomId + "'");

    internal static void Delete(IDbConnection dbClient, int roomId, int userId) => dbClient.Execute(
        "DELETE FROM room_right WHERE user_id = @UserId AND room_id = @RoomId LIMIT 1",
        new { UserId = userId, RoomId = roomId });

    internal static List<int> GetAllByRoomId(IDbConnection dbClient, int roomId) => dbClient.Query<int>(
        "SELECT user_id FROM `room_right` WHERE room_id = '" + roomId + "'"
    ).ToList();

    internal static List<int> GetAllByUserId(IDbConnection dbClient, int userId) => dbClient.Query<int>(
        "SELECT room_id FROM `room_right` WHERE user_id = '" + userId + "'"
    ).ToList();

    internal static void DeleteAll(IDbConnection dbClient, int roomId, List<int> userIds) => dbClient.Execute(
        "DELETE FROM `room_right` WHERE room_id = '" + roomId + "' AND user_id = @UserId",
        new { UserId = userIds });
}