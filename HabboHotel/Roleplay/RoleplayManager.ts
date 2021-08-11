class RolePlayerManager {}
class RPItemManager {}
class RPWeaponManager {}
class RPEnemyManager {}
class RPTrocManager {}

export class RoleplayManager
    {
        private _rolePlay: Map<number, RolePlayerManager>;
        private _roleplayItemManager: RPItemManager;
        private _roleplayWeaponManager: RPWeaponManager;
        private _roleplayEnemyManager: RPEnemyManager;
        private _roleplayTrocManager: RPTrocManager;

        public RoleplayManager()
        {
            this._rolePlay = new Map();

            this._roleplayItemManager = new RPItemManager();
            this._roleplayWeaponManager = new RPWeaponManager();
            this._roleplayEnemyManager = new RPEnemyManager();
            this._roleplayTrocManager = new RPTrocManager();
        }

        public GetRolePlay(Ownerid: number): RolePlayerManager
        {
            if (!this._rolePlay.has(Ownerid))
            {
                return null;
            }

            const RP = this._rolePlay.get(Ownerid);
            return RP;
        }

        public GetTrocManager(): RPTrocManager
        {
            return this._roleplayTrocManager;
        }

        public GetWeaponManager(): RPWeaponManager
        {
            return this._roleplayWeaponManager;
        }

        public GetItemManager(): RPItemManager
        {
            return this._roleplayItemManager;
        }

        public GetEnemyManager(): RPEnemyManager
        {
            return this._roleplayEnemyManager;
        }

        public Init(): void
        {
            // this._roleplayItemManager.Init();
            // this._roleplayWeaponManager.Init();
            // this._roleplayEnemyManager.Init();

            // using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            // {
            //     dbClient.SetQuery("SELECT owner_id, hopital_id, prison_id FROM roleplay");
            //     DataTable table1 = dbClient.GetTable();
            //     if (table1 != null)
            //     {
            //         foreach (DataRow dataRow in table1.Rows)
            //         {
            //             if (!this._rolePlay.ContainsKey(Convert.ToInt32(dataRow["owner_id"])))
            //             {
            //                 this._rolePlay.TryAdd(Convert.ToInt32(dataRow["owner_id"]), new RolePlayerManager(Convert.ToInt32(dataRow["owner_id"]), Convert.ToInt32(dataRow["hopital_id"]), Convert.ToInt32(dataRow["prison_id"])));
            //             }
            //             else
            //             {
            //                 this.GetRolePlay(Convert.ToInt32(dataRow["owner_id"])).Update(Convert.ToInt32(dataRow["hopital_id"]), Convert.ToInt32(dataRow["prison_id"]));
            //             }
            //         }
            //     }
            // }
        }
    }