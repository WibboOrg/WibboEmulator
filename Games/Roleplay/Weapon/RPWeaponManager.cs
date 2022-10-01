using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using System.Data;

namespace WibboEmulator.Game.Roleplay.Weapon
{
    public class RPWeaponManager
    {
        private readonly Dictionary<int, RPWeapon> _weaponCac;
        private readonly Dictionary<int, RPWeapon> _weaponGun;

        public RPWeaponManager()
        {
            this._weaponCac = new Dictionary<int, RPWeapon>();
            this._weaponGun = new Dictionary<int, RPWeapon>();
        }

        public RPWeapon GetWeaponCac(int Id)
        {
            RPWeapon weapon = new RPWeapon(0, 1, 3, RPWeaponInteraction.NONE, 0, 1, 1);
            if (!this._weaponCac.ContainsKey(Id))
            {
                return weapon;
            }

            this._weaponCac.TryGetValue(Id, out weapon);
            return weapon;
        }

        public RPWeapon GetWeaponGun(int Id)
        {
            RPWeapon weapon = new RPWeapon(0, 5, 10, RPWeaponInteraction.NONE, 164, 3, 10);
            if (!this._weaponGun.ContainsKey(Id))
            {
                return weapon;
            }

            this._weaponGun.TryGetValue(Id, out weapon);
            return weapon;
        }

        public void Init(IQueryAdapter dbClient)
        {
            this._weaponCac.Clear();
            this._weaponGun.Clear();

            DataTable table = RoleplayWeaponDao.GetAll(dbClient);
            if (table != null)
            {
                foreach (DataRow dataRow in table.Rows)
                {
                    if (this._weaponCac.ContainsKey(Convert.ToInt32(dataRow["id"])) || this._weaponGun.ContainsKey(Convert.ToInt32(dataRow["id"])))
                    {
                        continue;
                    }

                    if ((string)dataRow["type"] == "cac")
                    {
                        this._weaponCac.Add(Convert.ToInt32(dataRow["id"]), new RPWeapon(Convert.ToInt32(dataRow["id"]), Convert.ToInt32(dataRow["domage_min"]), Convert.ToInt32(dataRow["domage_max"]), RPWeaponInteractions.GetTypeFromString((string)dataRow["interaction"]), Convert.ToInt32(dataRow["enable"]), Convert.ToInt32(dataRow["freeze_time"]), Convert.ToInt32(dataRow["distance"])));
                    }
                    else
                    {
                        this._weaponGun.Add(Convert.ToInt32(dataRow["id"]), new RPWeapon(Convert.ToInt32(dataRow["id"]), Convert.ToInt32(dataRow["domage_min"]), Convert.ToInt32(dataRow["domage_max"]), RPWeaponInteractions.GetTypeFromString((string)dataRow["interaction"]), Convert.ToInt32(dataRow["enable"]), Convert.ToInt32(dataRow["freeze_time"]), Convert.ToInt32(dataRow["distance"])));
                    }
                }
            }
        }
    }
}
