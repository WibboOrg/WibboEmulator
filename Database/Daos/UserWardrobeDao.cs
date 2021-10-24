using Butterfly.Database;
using Butterfly.Database.Interfaces;
using System;

namespace Butterfly.Database.Daos
{
    class UserWardrobeDao
    {
        internal static void InsertLook(IQueryAdapter dbClient, int userId, int slotId, string look, string gender)
        {
            dbClient.SetQuery("SELECT null FROM user_wardrobe WHERE user_id = '" + userId + "' AND slot_id = '" + slotId + "';");
            dbClient.AddParameter("look", look);
            dbClient.AddParameter("gender", gender);
            if (dbClient.GetRow() != null)
            {
                dbClient.SetQuery("UPDATE user_wardrobe SET look = @look, gender = @gender WHERE user_id = " + userId + " AND slot_id = " + slotId + ";");
                dbClient.AddParameter("look", look);
                dbClient.AddParameter("gender", gender);
                dbClient.RunQuery();
            }
            else
            {
                dbClient.SetQuery("INSERT INTO user_wardrobe (user_id,slot_id,look,gender) VALUES (" + userId + "," + slotId + ",@look,@gender)");
                dbClient.AddParameter("look", look);
                dbClient.AddParameter("gender", gender);
                dbClient.RunQuery();
            }
        }
    }
}
