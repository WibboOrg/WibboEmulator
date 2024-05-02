namespace WibboEmulator.Games.Roleplays.Weapon;
using System.Data;
using WibboEmulator.Database.Daos.Roleplay;
using WibboEmulator.Utilities;

public static class RPWeaponManager
{
    private static readonly Dictionary<int, RPWeapon> WeaponCac = new();
    private static readonly Dictionary<int, RPWeapon> WeaponGun = new();

    public static RPWeapon GetWeaponCac(int id)
    {
        var weapon = new RPWeapon(0, 1, 3, RPWeaponInteraction.None, 0, 1, 1);
        if (!WeaponCac.ContainsKey(id))
        {
            return weapon;
        }

        _ = WeaponCac.TryGetValue(id, out weapon);
        return weapon;
    }

    public static RPWeapon GetWeaponGun(int id)
    {
        var weapon = new RPWeapon(0, 5, 10, RPWeaponInteraction.None, 164, 3, 10);
        if (!WeaponGun.ContainsKey(id))
        {
            return weapon;
        }

        _ = WeaponGun.TryGetValue(id, out weapon);
        return weapon;
    }

    public static void Initialize(IDbConnection dbClient)
    {
        WeaponCac.Clear();
        WeaponGun.Clear();

        var weaponList = RoleplayWeaponDao.GetAll(dbClient);
        if (weaponList.Count != 0)
        {
            foreach (var weapon in weaponList)
            {
                if (WeaponCac.ContainsKey(weapon.Id) || WeaponGun.ContainsKey(weapon.Id))
                {
                    continue;
                }

                var rpWeapon = new RPWeapon(weapon.Id, weapon.DomageMin, weapon.DomageMax, weapon.Interaction.ToEnum(RPWeaponInteraction.None), weapon.Enable, weapon.FreezeTime, weapon.Distance);

                if (weapon.Type == "cac")
                {
                    WeaponCac.Add(weapon.Id, rpWeapon);
                }
                else
                {
                    WeaponGun.Add(weapon.Id, rpWeapon);
                }
            }
        }
    }
}
