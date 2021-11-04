using System.Data;
using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class UserRoleplayDao
    {
        internal static void Delete(IQueryAdapter dbClient, int userId, int roleplayId)
        {
            dbClient.RunQuery("DELETE FROM user_rp WHERE user_id = '" + userId + "' AND roleplay_id = '" + roleplayId + "'");
        }

        internal static void Update(IQueryAdapter dbClient, int userId, int roleplayId, int health, int energy, int hygiene, int money, int money1, int money2, int money3, int money4, int munition, int exp, int weaponGunId, int weaponCacId)
        {
            dbClient.RunQuery("UPDATE user_rp SET health='" + health + "', energy='" + energy + "', hygiene='" + hygiene + "', money='" + money + "', money_1='" + money1 + "', money_2='" + money2 + "', money_3='" + money3 + "', money_4='" + money4 + "', munition='" + munition + "', exp='" + exp + "', weapon_far='" + weaponGunId + "', weapon_cac='" + weaponCacId + "' WHERE user_id='" + userId + "' AND roleplay_id = '" + roleplayId + "' LIMIT 1");
        }

        internal static DataRow GetOne(IQueryAdapter dbClient, int userId, int roleplayId)
        {
            dbClient.SetQuery("SELECT * FROM user_rp WHERE user_id = '" + userId + "' AND roleplay_id = '" + roleplayId + "'");
            return dbClient.GetRow();
        }

        internal static void Insert(IQueryAdapter dbClient, int userId, int roleplayId)
        {
            dbClient.RunQuery("INSERT INTO user_rp (user_id, roleplay_id) VALUES ('" + userId + "', '" + roleplayId + "')");
        }
    }
}