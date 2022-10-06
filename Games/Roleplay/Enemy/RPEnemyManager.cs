namespace WibboEmulator.Games.Roleplay.Enemy;
using System.Data;
using WibboEmulator.Database.Daos.Roleplay;
using WibboEmulator.Database.Interfaces;

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

    public void Init(IQueryAdapter dbClient)
    {
        this._enemyBot.Clear();
        this._enemyPet.Clear();

        var table = RoleplayEnemyDao.GetAll(dbClient);
        if (table != null)
        {
            foreach (DataRow dataRow in table.Rows)
            {
                if ((this._enemyBot.ContainsKey(Convert.ToInt32(dataRow["id"])) && (string)dataRow["type"] == "bot") || (this._enemyPet.ContainsKey(Convert.ToInt32(dataRow["id"])) && (string)dataRow["type"] == "pet"))
                {
                    continue;
                }

                var config = new RPEnemy(Convert.ToInt32(dataRow["id"]), Convert.ToInt32(dataRow["health"]), Convert.ToInt32(dataRow["weapon_far_id"]), Convert.ToInt32(dataRow["weapon_cac_id"]), Convert.ToInt32(dataRow["dead_timer"]),
                    Convert.ToInt32(dataRow["loot_item_id"]), Convert.ToInt32(dataRow["money_drop"]), Convert.ToInt32((string)dataRow["drop_script_id"]), Convert.ToInt32(dataRow["team_id"]), Convert.ToInt32(dataRow["aggro_distance"]),
                    Convert.ToInt32(dataRow["zone_distance"]), Convert.ToInt32((string)dataRow["reset_position"]) == 1, Convert.ToInt32(dataRow["lost_aggro_distance"]), Convert.ToInt32((string)dataRow["zombie_mode"]) == 1);

                if ((string)dataRow["type"] == "bot")
                {
                    this._enemyBot.Add(Convert.ToInt32(dataRow["id"]), config);
                }
                else
                {
                    this._enemyPet.Add(Convert.ToInt32(dataRow["id"]), config);
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

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
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

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
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

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
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

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            RoleplayEnemyDao.Delete(dbClient, petId);
        }

        _ = this._enemyPet.Remove(petId);
    }
}
