using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class UserBadgeDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE user_badges SET badge_slot = '0' WHERE user_id = '" + Session.GetHabbo().Id + "' AND badge_slot != '0'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE user_badges SET badge_slot = '" + Slot + "' WHERE badge_id = @badge AND user_id = '" + Session.GetHabbo().Id + "'");
            dbClient.AddParameter("badge", Badge);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO user_badges (user_id,badge_id,badge_slot) VALUES (" + this._userId + ",@badge," + Slot + ")");
            dbClient.AddParameter("badge", Badge);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("DELETE FROM user_badges WHERE badge_id = @badge AND user_id = '" + this._userId + "' LIMIT 1");
            dbClient.AddParameter("badge", Badge);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("DELETE FROM user_badges WHERE badge_id = @badge AND user_id = '" + this._userId + "' LIMIT 1");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("DELETE FROM user_badges WHERE badge_id = '" + this._badges + "'");
            dbClient.AddParameter("badge", Badge);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM user_badges WHERE user_id = '" + userId + "';");
            Badges = dbClient.GetTable();
        }
    }
}