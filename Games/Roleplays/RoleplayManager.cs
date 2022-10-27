namespace WibboEmulator.Games.Roleplays;
using System.Collections.Concurrent;
using System.Data;
using WibboEmulator.Database.Daos.Roleplay;
using WibboEmulator.Database.Interfaces;
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

    public void Init(IQueryAdapter dbClient)
    {
        this.ItemManager.Init(dbClient);
        this.WeaponManager.Init(dbClient);
        this.EnemyManager.Init(dbClient);

        var table = RoleplayDao.GetAll(dbClient);
        if (table != null)
        {
            foreach (DataRow dataRow in table.Rows)
            {
                if (!this._rolePlay.ContainsKey(Convert.ToInt32(dataRow["owner_id"])))
                {
                    _ = this._rolePlay.TryAdd(Convert.ToInt32(dataRow["owner_id"]), new RolePlayerManager(Convert.ToInt32(dataRow["owner_id"]), Convert.ToInt32(dataRow["hopital_id"]), Convert.ToInt32(dataRow["prison_id"])));
                }
                else
                {
                    this.GetRolePlay(Convert.ToInt32(dataRow["owner_id"])).Update(Convert.ToInt32(dataRow["hopital_id"]), Convert.ToInt32(dataRow["prison_id"]));
                }
            }
        }
    }
}
