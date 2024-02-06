namespace WibboEmulator.Games.Roleplays.Enemy;
using System.Data;
using WibboEmulator.Database.Daos.Roleplay;

public class RPEnemyManager
{
    private readonly Dictionary<int, RPEnemy> _enemyBot;
    private readonly Dictionary<int, RPEnemy> _enemyPet;

    public RPEnemyManager()
    {
        this._enemyBot = new Dictionary<int, RPEnemy>();
        this._enemyPet = new Dictionary<int, RPEnemy>();
    }

    public RPEnemy GetEnemyBot(int id)
    {
        if (!this._enemyBot.ContainsKey(id))
        {
            return null;
        }

        _ = this._enemyBot.TryGetValue(id, out var enemy);
        return enemy;
    }

    public RPEnemy GetEnemyPet(int id)
    {
        if (!this._enemyPet.ContainsKey(id))
        {
            return null;
        }

        _ = this._enemyPet.TryGetValue(id, out var enemy);
        return enemy;
    }

    public void Initialize(IDbConnection dbClient)
    {
        this._enemyBot.Clear();
        this._enemyPet.Clear();

        var enemyList = RoleplayEnemyDao.GetAll(dbClient);
        if (enemyList.Count != 0)
        {
            foreach (var enemy in enemyList)
            {
                if ((this._enemyBot.ContainsKey(enemy.Id) && enemy.Type == "bot") || (this._enemyPet.ContainsKey(enemy.Id) && enemy.Type == "pet"))
                {
                    continue;
                }

                var config = new RPEnemy(enemy.Id, enemy.Health, enemy.WeaponFarId, enemy.WeaponCacId, enemy.DeadTimer,
                    enemy.LootItemId, enemy.MoneyDrop, enemy.DropScriptId, enemy.TeamId, enemy.AggroDistance,
                    enemy.ZoneDistance, enemy.ResetPosition, enemy.LostAggroDistance, enemy.ZombieMode);

                if (enemy.Type == "bot")
                {
                    this._enemyBot.Add(enemy.Id, config);
                }
                else
                {
                    this._enemyPet.Add(enemy.Id, config);
                }
            }
        }
    }

    public RPEnemy AddEnemyBot(int botId)
    {
        if (this._enemyBot.ContainsKey(botId))
        {
            return this.GetEnemyBot(botId);
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().Connection())
        {
            RoleplayEnemyDao.Insert(dbClient, botId, "bot");
        }

        var enemyConfig = new RPEnemy(botId, 100, 1, 4, 30, 0, 0, 5461, 0, 0, 0, true, 12, false);
        this._enemyBot.Add(botId, enemyConfig);
        return this.GetEnemyBot(botId);
    }

    public RPEnemy AddEnemyPet(int petId)
    {
        if (this._enemyPet.ContainsKey(petId))
        {
            return this.GetEnemyPet(petId);
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().Connection())
        {
            RoleplayEnemyDao.Insert(dbClient, petId, "pet");
        }

        var enemyConfig = new RPEnemy(petId, 100, 0, 0, 0, 0, 0, 5461, 0, 0, 0, true, 12, false);
        this._enemyPet.Add(petId, enemyConfig);
        return this.GetEnemyPet(petId);
    }

    internal void RemoveEnemyBot(int botId)
    {
        if (!this._enemyBot.ContainsKey(botId))
        {
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().Connection())
        {
            RoleplayEnemyDao.Delete(dbClient, botId);
        }

        _ = this._enemyBot.Remove(botId);
    }

    internal void RemoveEnemyPet(int petId)
    {
        if (!this._enemyPet.ContainsKey(petId))
        {
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().Connection())
        {
            RoleplayEnemyDao.Delete(dbClient, petId);
        }

        _ = this._enemyPet.Remove(petId);
    }
}
