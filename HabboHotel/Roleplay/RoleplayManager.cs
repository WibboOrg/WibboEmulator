using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.Roleplay.Enemy;
using Butterfly.HabboHotel.Roleplay.Troc;
using Butterfly.HabboHotel.Roleplay.Weapon;
using System;
using System.Collections.Concurrent;
using System.Data;

namespace Butterfly.HabboHotel.Roleplay
{
    public class RoleplayManager
    {
        private readonly ConcurrentDictionary<int, RolePlayerManager> _rolePlay;
        private readonly RPItemManager _roleplayItemManager;
        private readonly RPWeaponManager _roleplayWeaponManager;
        private readonly RPEnemyManager _roleplayEnemyManager;
        private readonly RPTrocManager _roleplayTrocManager;

        public RoleplayManager()
        {
            this._rolePlay = new ConcurrentDictionary<int, RolePlayerManager>();

            this._roleplayItemManager = new RPItemManager();
            this._roleplayWeaponManager = new RPWeaponManager();
            this._roleplayEnemyManager = new RPEnemyManager();
            this._roleplayTrocManager = new RPTrocManager();
        }

        public RolePlayerManager GetRolePlay(int Ownerid)
        {
            if (!this._rolePlay.ContainsKey(Ownerid))
            {
                return null;
            }

            this._rolePlay.TryGetValue(Ownerid, out RolePlayerManager RP);
            return RP;
        }

        public RPTrocManager GetTrocManager()
        {
            return this._roleplayTrocManager;
        }

        public RPWeaponManager GetWeaponManager()
        {
            return this._roleplayWeaponManager;
        }

        public RPItemManager GetItemManager()
        {
            return this._roleplayItemManager;
        }

        public RPEnemyManager GetEnemyManager()
        {
            return this._roleplayEnemyManager;
        }

        public void Init()
        {
            this._roleplayItemManager.Init();
            this._roleplayWeaponManager.Init();
            this._roleplayEnemyManager.Init();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT owner_id, hopital_id, prison_id FROM roleplay");
                DataTable table1 = dbClient.GetTable();
                if (table1 != null)
                {
                    foreach (DataRow dataRow in table1.Rows)
                    {
                        if (!this._rolePlay.ContainsKey(Convert.ToInt32(dataRow["owner_id"])))
                        {
                            this._rolePlay.TryAdd(Convert.ToInt32(dataRow["owner_id"]), new RolePlayerManager(Convert.ToInt32(dataRow["owner_id"]), Convert.ToInt32(dataRow["hopital_id"]), Convert.ToInt32(dataRow["prison_id"])));
                        }
                        else
                        {
                            this.GetRolePlay(Convert.ToInt32(dataRow["owner_id"])).Update(Convert.ToInt32(dataRow["hopital_id"]), Convert.ToInt32(dataRow["prison_id"]));
                        }
                    }
                }
            }
        }
    }
}
