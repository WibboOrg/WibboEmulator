namespace WibboEmulator.Games.Roleplays;
using System.Collections.Concurrent;
using System.Data;
using WibboEmulator.Database.Daos.Roleplay;
using WibboEmulator.Games.Roleplays.Enemy;
using WibboEmulator.Games.Roleplays.Item;
using WibboEmulator.Games.Roleplays.Player;
using WibboEmulator.Games.Roleplays.Troc;
using WibboEmulator.Games.Roleplays.Weapon;

public class RoleplayManager
{
    private readonly ConcurrentDictionary<int, RolePlayerManager> _rolePlay;

    public RPTrocManager TrocManager { get; }
    public RPWeaponManager WeaponManager { get; }
    public RPItemManager ItemManager { get; }
    public RPEnemyManager EnemyManager { get; }

    public RoleplayManager()
    {
        this._rolePlay = new ConcurrentDictionary<int, RolePlayerManager>();

        this.ItemManager = new RPItemManager();
        this.WeaponManager = new RPWeaponManager();
        this.EnemyManager = new RPEnemyManager();
        this.TrocManager = new RPTrocManager();
    }

    public RolePlayerManager GetRolePlay(int ownerId)
    {
        if (!this._rolePlay.ContainsKey(ownerId))
        {
            return null;
        }

        _ = this._rolePlay.TryGetValue(ownerId, out var rp);
        return rp;
    }

    public void Initialize(IDbConnection dbClient)
    {
        this._rolePlay.Clear();

        this.ItemManager.Initialize(dbClient);
        this.WeaponManager.Initialize(dbClient);
        this.EnemyManager.Initialize(dbClient);

        var roleplayList = RoleplayDao.GetAll(dbClient);
        if (roleplayList.Count != 0)
        {
            foreach (var roleplay in roleplayList)
            {
                if (!this._rolePlay.ContainsKey(roleplay.OwnerId))
                {
                    _ = this._rolePlay.TryAdd(roleplay.OwnerId, new RolePlayerManager(roleplay.OwnerId, roleplay.HopitalId, roleplay.PrisonId));
                }
            }
        }
    }
}
