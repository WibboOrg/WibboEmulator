namespace WibboEmulator.Games.Roleplays;
using System.Collections.Concurrent;
using System.Data;
using WibboEmulator.Database.Daos.Roleplay;
using WibboEmulator.Games.Roleplays.Enemy;
using WibboEmulator.Games.Roleplays.Item;
using WibboEmulator.Games.Roleplays.Player;
using WibboEmulator.Games.Roleplays.Weapon;

public static class RoleplayManager
{
    private static readonly ConcurrentDictionary<int, RolePlayerManager> RolePlays = new();

    public static RolePlayerManager GetRolePlay(int ownerId)
    {
        if (!RolePlays.ContainsKey(ownerId))
        {
            return null;
        }

        _ = RolePlays.TryGetValue(ownerId, out var rp);
        return rp;
    }

    public static void Initialize(IDbConnection dbClient)
    {
        RolePlays.Clear();

        RPItemManager.Initialize(dbClient);
        RPWeaponManager.Initialize(dbClient);
        RPEnemyManager.Initialize(dbClient);

        var roleplayList = RoleplayDao.GetAll(dbClient);
        if (roleplayList.Count != 0)
        {
            foreach (var roleplay in roleplayList)
            {
                if (!RolePlays.ContainsKey(roleplay.OwnerId))
                {
                    _ = RolePlays.TryAdd(roleplay.OwnerId, new RolePlayerManager(roleplay.OwnerId, roleplay.HopitalId, roleplay.PrisonId));
                }
            }
        }
    }
}
