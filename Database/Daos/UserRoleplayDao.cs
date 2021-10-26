using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class UserRoleplayDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM user_rp WHERE user_id = '" + this._id + "' AND roleplay_id = '" + this._rpId + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE user_rp SET health='" + this.Health + "', energy='" + this.Energy + "', hygiene='" + this.Hygiene + "', money='" + this.Money + "', money_1='" + this.Money1 + "', money_2='" + this.Money2 + "', money_3='" + this.Money3 + "', money_4='" + this.Money4 + "', munition='" + this.Munition + "', exp='" + this.Exp + "', weapon_far='" + this.WeaponGun.Id + "', weapon_cac='" + this.WeaponCac.Id + "' WHERE user_id='" + this._id + "' AND roleplay_id = '" + this._rpId + "' LIMIT 1");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE user_rp SET health='" + this.Health + "', energy='" + this.Energy + "', hygiene='" + this.Hygiene + "', money='" + this.Money + "', money_1='" + this.Money1 + "', money_2='" + this.Money2 + "', money_3='" + this.Money3 + "', money_4='" + this.Money4 + "', munition='" + this.Munition + "', exp='" + this.Exp + "', weapon_far='" + this.WeaponGun.Id + "', weapon_cac='" + this.WeaponCac.Id + "' WHERE user_id='" + this._id + "' AND roleplay_id = '" + this._rpId + "' LIMIT 1");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM user_rp WHERE user_id = '" + UserId + "' AND roleplay_id = '" + this._id + "'");
            DataRow dRow = dbClient.GetRow();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("INSERT INTO user_rp (user_id, roleplay_id) VALUES ('" + UserId + "', '" + this._id + "')");
        }
    }
}