namespace WibboEmulator.Games.Roleplays.Enemy;
using System.Data;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Roleplay;

public static class RPEnemyManager
{
    private static readonly Dictionary<int, RPEnemy> EnemyBot = new();
    private static readonly Dictionary<int, RPEnemy> EnemyPet = new();

    public static RPEnemy GetEnemyBot(int id)
    {
        if (!EnemyBot.ContainsKey(id))
        {
            return null;
        }

        _ = EnemyBot.TryGetValue(id, out var enemy);
        return enemy;
    }

    public static RPEnemy GetEnemyPet(int id)
    {
        if (!EnemyPet.ContainsKey(id))
        {
            return null;
        }

        _ = EnemyPet.TryGetValue(id, out var enemy);
        return enemy;
    }

    public static void Initialize(IDbConnection dbClient)
    {
        EnemyBot.Clear();
        EnemyPet.Clear();

        var enemyList = RoleplayEnemyDao.GetAll(dbClient);
        if (enemyList.Count != 0)
        {
            foreach (var enemy in enemyList)
            {
                if ((EnemyBot.ContainsKey(enemy.Id) && enemy.Type == "bot") || (EnemyPet.ContainsKey(enemy.Id) && enemy.Type == "pet"))
                {
                    continue;
                }

                var config = new RPEnemy(enemy.Id, enemy.Health, enemy.WeaponFarId, enemy.WeaponCacId, enemy.DeadTimer,
                    enemy.LootItemId, enemy.MoneyDrop, enemy.DropScriptId, enemy.TeamId, enemy.AggroDistance,
                    enemy.ZoneDistance, enemy.ResetPosition, enemy.LostAggroDistance, enemy.ZombieMode);

                if (enemy.Type == "bot")
                {
                    EnemyBot.Add(enemy.Id, config);
                }
                else
                {
                    EnemyPet.Add(enemy.Id, config);
                }
            }
        }
    }

    public static RPEnemy AddEnemyBot(int botId)
    {
        if (EnemyBot.ContainsKey(botId))
        {
            return GetEnemyBot(botId);
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            RoleplayEnemyDao.Insert(dbClient, botId, "bot");
        }

        var enemyConfig = new RPEnemy(botId, 100, 1, 4, 30, 0, 0, 5461, 0, 0, 0, true, 12, false);
        EnemyBot.Add(botId, enemyConfig);
        return GetEnemyBot(botId);
    }

    public static RPEnemy AddEnemyPet(int petId)
    {
        if (EnemyPet.ContainsKey(petId))
        {
            return GetEnemyPet(petId);
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            RoleplayEnemyDao.Insert(dbClient, petId, "pet");
        }

        var enemyConfig = new RPEnemy(petId, 100, 0, 0, 0, 0, 0, 5461, 0, 0, 0, true, 12, false);
        EnemyPet.Add(petId, enemyConfig);
        return GetEnemyPet(petId);
    }

    internal static void RemoveEnemyBot(int botId)
    {
        if (!EnemyBot.ContainsKey(botId))
        {
            return;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            RoleplayEnemyDao.Delete(dbClient, botId);
        }

        _ = EnemyBot.Remove(botId);
    }

    internal static void RemoveEnemyPet(int petId)
    {
        if (!EnemyPet.ContainsKey(petId))
        {
            return;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            RoleplayEnemyDao.Delete(dbClient, petId);
        }

        _ = EnemyPet.Remove(petId);
    }
}
