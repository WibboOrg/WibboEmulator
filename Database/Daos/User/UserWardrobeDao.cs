using Wibbo.Database.Interfaces;
using System.Data;

namespace Wibbo.Database.Daos
{
    class UserWardrobeDao
    {
        internal static void Insert(IQueryAdapter dbClient, int userId, int slotId, string look, string gender)
        {
            dbClient.SetQuery("REPLACE INTO `user_wardrobe` (`user_id`, `slot_id`, `look`, `gender`) VALUES (" + userId + "," + slotId + ",@look,@gender)");
            dbClient.AddParameter("look", look);
            dbClient.AddParameter("gender", gender);
            dbClient.RunQuery();
        }

        internal static DataTable GetAll(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT `slot_id`, `look`, `gender` FROM `user_wardrobe` WHERE `user_id` = '" + userId + "' LIMIT 24");
            return dbClient.GetTable();
        }

        internal static string GetOneRandomLook(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT `look` FROM `user_wardrobe` WHERE `user_id` IN (SELECT `user_id` FROM (SELECT `user_id` FROM `user_wardrobe` WHERE `user_id` >= ROUND(RAND() * (SELECT max(`user_id`) FROM `user_wardrobe`)) LIMIT 1) tmp) ORDER BY RAND() LIMIT 1");
            return dbClient.GetString();
        }
    }
}
