using Wibbo.Database.Interfaces;
namespace Wibbo.Database.Daos
{
    class UserPhotoDao
    {
        internal static void Insert(IQueryAdapter dbClient, int userId, string photoId, int time)
        {
            dbClient.SetQuery("INSERT INTO `user_photo` (`user_id`, `photo`, `time`) VALUES ('" + userId + "', @photoid, '" + time + "')");
            dbClient.AddParameter("photoid", photoId);
            dbClient.RunQuery();
        }
    }
}
