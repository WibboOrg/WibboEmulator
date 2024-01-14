namespace WibboEmulator.Database.Daos.User;

using System.Data;
using Dapper;

internal sealed class UserWardrobeDao
{
    internal static void Insert(IDbConnection dbClient, int userId, int slotId, string look, string gender) => dbClient.Execute(
        "REPLACE INTO user_wardrobe (user_id, slot_id, look, gender) VALUES (@UserId, @SlotId, @Look, @Gender)",
        new { UserId = userId, SlotId = slotId, Look = look, Gender = gender });

    internal static List<UserWardrobeEntity> GetAll(IDbConnection dbClient, int userId) => dbClient.Query<UserWardrobeEntity>(
        "SELECT `slot_id`, `look`, `gender` FROM `user_wardrobe` WHERE `user_id` = '" + userId + "' LIMIT 24"
    ).ToList();

    internal static string GetOneRandomLook(IDbConnection dbClient) => dbClient.QuerySingleOrDefault<string>(
        "SELECT `look` FROM `user_wardrobe` WHERE `user_id` IN (SELECT `user_id` FROM (SELECT `user_id` FROM `user_wardrobe` WHERE `user_id` >= ROUND(RAND() * (SELECT max(`user_id`) FROM `user_wardrobe`)) LIMIT 1) tmp) ORDER BY RAND() LIMIT 1");
}

public class UserWardrobeEntity
{
    public int UserId { get; set; }
    public int SlotId { get; set; }
    public string Look { get; set; }
    public string Gender { get; set; }
}