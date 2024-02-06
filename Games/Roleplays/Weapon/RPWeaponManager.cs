namespace WibboEmulator.Games.Roleplays.Weapon;
using System.Data;
using WibboEmulator.Database.Daos.Roleplay;
using WibboEmulator.Utilities;

public class RPWeaponManager
{
    private readonly Dictionary<int, RPWeapon> _weaponCac;
    private readonly Dictionary<int, RPWeapon> _weaponGun;

    public RPWeaponManager()
    {
        this._weaponCac = new Dictionary<int, RPWeapon>();
        this._weaponGun = new Dictionary<int, RPWeapon>();
    }

    public RPWeapon GetWeaponCac(int id)
    {
        var weapon = new RPWeapon(0, 1, 3, RPWeaponInteraction.None, 0, 1, 1);
        if (!this._weaponCac.ContainsKey(id))
        {
            return weapon;
        }

        _ = this._weaponCac.TryGetValue(id, out weapon);
        return weapon;
    }

    public RPWeapon GetWeaponGun(int id)
    {
        var weapon = new RPWeapon(0, 5, 10, RPWeaponInteraction.None, 164, 3, 10);
        if (!this._weaponGun.ContainsKey(id))
        {
            return weapon;
        }

        _ = this._weaponGun.TryGetValue(id, out weapon);
        return weapon;
    }

    public void Initialize(IDbConnection dbClient)
    {
        this._weaponCac.Clear();
        this._weaponGun.Clear();

        var weaponList = RoleplayWeaponDao.GetAll(dbClient);
        if (weaponList.Count != 0)
        {
            foreach (var weapon in weaponList)
            {
                if (this._weaponCac.ContainsKey(weapon.Id) || this._weaponGun.ContainsKey(weapon.Id))
                {
                    continue;
                }

                var rpWeapon = new RPWeapon(weapon.Id, weapon.DomageMin, weapon.DomageMax, weapon.Interaction.ToEnum(RPWeaponInteraction.None), weapon.Enable, weapon.FreezeTime, weapon.Distance);

                if (weapon.Type == "cac")
                {
                    this._weaponCac.Add(weapon.Id, rpWeapon);
                }
                else
                {
                    this._weaponGun.Add(weapon.Id, rpWeapon);
                }
            }
        }
    }
}
