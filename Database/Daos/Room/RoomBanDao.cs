namespace WibboEmulator.Database.Daos.Room;
using System.Data;
using Dapper;

internal sealed class RoomBanDao
{
    internal static void Insert(IDbConnection dbClient, int roomId, int userId, int expire) => dbClient.Execute(
        "INSERT INTO `room_ban` (`room_id`, `user_id`, `expire`) VALUES ('" + roomId + "', '" + userId + "', '" + expire + "')");

    internal static void Delete(IDbConnection dbClient, int roomId) => dbClient.Execute(
        "DELETE FROM `room_ban` WHERE `room_id` = '" + roomId + "'");

    internal static void Delete(IDbConnection dbClient, int roomId, int userId) => dbClient.Execute(
        "DELETE FROM room_ban WHERE user_id = @UserId AND `room_id` = @RoomId LIMIT 1",
        new { UserId = userId, RoomId = roomId });

    internal static List<RoomBanEntity> GetAllByRoomId(IDbConnection dbClient, int roomId) => dbClient.Query<RoomBanEntity>(
        "SELECT `user_id`, `expire` FROM `room_ban` WHERE `room_id` = '" + roomId + "'"
    ).ToList();
}

public class RoomBanEntity
{
    public int UserId { get; set; }
    public int Expire { get; set; }
}
