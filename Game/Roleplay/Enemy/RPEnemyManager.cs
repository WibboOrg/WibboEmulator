using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Roleplay.Enemy
{
    public class RPEnemyManager
    {
        private readonly Dictionary<int, RPEnemy> _enemyBot;
        private readonly Dictionary<int, RPEnemy> _enemyPet;

        public RPEnemyManager()
        {
            this._enemyBot = new Dictionary<int, RPEnemy>();
            this._enemyPet = new Dictionary<int, RPEnemy>();
        }

        public RPEnemy GetEnemyBot(int Id)
        {
            if (!this._enemyBot.ContainsKey(Id))
            {
                return null;
            }

            this._enemyBot.TryGetValue(Id, out RPEnemy enemy);
            return enemy;
        }

        public RPEnemy GetEnemyPet(int Id)
        {
            if (!this._enemyPet.ContainsKey(Id))
            {
                return null;
            }

            this._enemyPet.TryGetValue(Id, out RPEnemy enemy);
            return enemy;
        }

        public void Init(IQueryAdapter dbClient)
        {
            this._enemyBot.Clear();
            this._enemyPet.Clear();

            DataTable table = RoleplayEnemyDao.GetAll(dbClient);
            if (table != null)
            {
                foreach (DataRow dataRow in table.Rows)
                {
                    if ((this._enemyBot.ContainsKey(Convert.ToInt32(dataRow["id"])) && (string)dataRow["type"] == "bot") || (this._enemyPet.ContainsKey(Convert.ToInt32(dataRow["id"])) && (string)dataRow["type"] == "pet"))
                    {
                        continue;
                    }

                    RPEnemy Config = new RPEnemy(Convert.ToInt32(dataRow["id"]), Convert.ToInt32(dataRow["health"]), Convert.ToInt32(dataRow["weapon_far_id"]), Convert.ToInt32(dataRow["weapon_cac_id"]), Convert.ToInt32(dataRow["dead_timer"]),
                        Convert.ToInt32(dataRow["loot_item_id"]), Convert.ToInt32(dataRow["money_drop"]), Convert.ToInt32((string)dataRow["drop_script_id"]), Convert.ToInt32(dataRow["team_id"]), Convert.ToInt32(dataRow["aggro_distance"]),
                        Convert.ToInt32(dataRow["zone_distance"]), Convert.ToInt32((string)dataRow["reset_position"]) == 1, Convert.ToInt32(dataRow["lost_aggro_distance"]), Convert.ToInt32((string)dataRow["zombie_mode"]) == 1);

                    if ((string)dataRow["type"] == "bot")
                    {
                        this._enemyBot.Add(Convert.ToInt32(dataRow["id"]), Config);
                    }
                    else
                    {
                        this._enemyPet.Add(Convert.ToInt32(dataRow["id"]), Config);
                    }
                }
            }
        }

        public RPEnemy AddEnemyBot(int BotId)
        {
            if (this._enemyBot.ContainsKey(BotId))
            {
                return this.GetEnemyBot(BotId);
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoleplayEnemyDao.Insert(dbClient, BotId, "bot");
            }

            RPEnemy EnemyConfig = new RPEnemy(BotId, 100, 1, 4, 30, 0, 0, 5461, 0, 0, 0, true, 12, false);
            this._enemyBot.Add(BotId, EnemyConfig);
            return this.GetEnemyBot(BotId);
        }

        public RPEnemy AddEnemyPet(int PetId)
        {
            if (this._enemyPet.ContainsKey(PetId))
            {
                return this.GetEnemyPet(PetId);
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoleplayEnemyDao.Insert(dbClient, PetId, "pet");
            }

            RPEnemy EnemyConfig = new RPEnemy(PetId, 100, 0, 0, 0, 0, 0, 5461, 0, 0, 0, true, 12, false);
            this._enemyPet.Add(PetId, EnemyConfig);
            return this.GetEnemyPet(PetId);
        }

        internal void RemoveEnemyBot(int BotId)
        {
            if (!this._enemyBot.ContainsKey(BotId))
            {
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoleplayEnemyDao.Delete(dbClient, BotId);
            }

            this._enemyBot.Remove(BotId);
        }

        internal void RemoveEnemyPet(int PetId)
        {
            if (!this._enemyPet.ContainsKey(PetId))
            {
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoleplayEnemyDao.Delete(dbClient, PetId);
            }

            this._enemyPet.Remove(PetId);
        }
    }
}
