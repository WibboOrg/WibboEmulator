namespace WibboEmulator.Database.Daos.User;
using System.Data;
using Dapper;

internal sealed class UserPhotoDao
{
    internal static void Insert(IDbConnection dbClient, int userId, string photoId, int time) => dbClient.Execute(
        "INSERT INTO user_photo (user_id, photo, time) VALUES (@UserId, @PhotoId, @Time)",
        new { UserId = userId, PhotoId = photoId, Time = time });
}
