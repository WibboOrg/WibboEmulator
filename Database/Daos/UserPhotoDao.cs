using Butterfly.Database.Interfaces;
namespace Butterfly.Database.Daos
{
    class UserPhotoDao
    {
        internal static void Insert(IQueryAdapter dbClient, int userId, string photoId, int time)
        {
            dbClient.SetQuery("INSERT INTO user_photos (user_id,photo,time) VALUES ('" + userId + "', @photoid, '" + time + "')");
            dbClient.AddParameter("photoid", photoId);
            dbClient.RunQuery();
        }
    }
}
