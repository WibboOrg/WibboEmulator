using System.Data;
using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class UserBadgeDao
    {
        internal static void UpdateSlot0(IQueryAdapter dbClient, int userId)
        {
            dbClient.RunQuery("UPDATE user_badges SET badge_slot = '0' WHERE user_id = '" + userId + "' AND badge_slot != '0'");
        }

        internal static void UpdateSlot(IQueryAdapter dbClient, int userId, int slot, string badge)
        {
            dbClient.SetQuery("UPDATE user_badges SET badge_slot = '" + slot + "' WHERE badge_id = @badge AND user_id = '" + userId + "'");
            dbClient.AddParameter("badge", badge);
            dbClient.RunQuery();
        }

        internal static void Insert(IQueryAdapter dbClient, int userId, int slot, string badge)
        {
            dbClient.SetQuery("INSERT INTO user_badges (user_id,badge_id,badge_slot) VALUES (" + userId + ",@badge," + slot + ")");
            dbClient.AddParameter("badge", badge);
            dbClient.RunQuery();
        }

        internal static void Delete(IQueryAdapter dbClient, int userId, string badge)
        {
            dbClient.SetQuery("DELETE FROM user_badges WHERE badge_id = @badge AND user_id = '" + userId + "' LIMIT 1");
            dbClient.AddParameter("badge", badge);
            dbClient.RunQuery();
        }

        internal static void Delete(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("DELETE FROM user_badges WHERE badge_id = @badge AND user_id = '" + userId + "' LIMIT 1");
        }

        internal static void DeleteAll(IQueryAdapter dbClient, int badgeId, string badge)
        {
            dbClient.SetQuery("DELETE FROM user_badges WHERE badge_id = '" + badgeId + "'");
            dbClient.AddParameter("badge", badge);
            dbClient.RunQuery();
        }

        internal static DataTable GetAll(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT * FROM user_badges WHERE user_id = '" + userId + "';");
            return dbClient.GetTable();
        }
    }
}