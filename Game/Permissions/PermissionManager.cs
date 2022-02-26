using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Permissions
{
    public class PermissionManager
    {
        private readonly Dictionary<string, int> _rights;

        private readonly Dictionary<string, PermissionCommand> _commands = new Dictionary<string, PermissionCommand>();

        public PermissionManager()
        {
            this._rights = new Dictionary<string, int>();
        }

        public void Init(IQueryAdapter dbClient)
        {
            this._rights.Clear();
            this._commands.Clear();

            DataTable table = EmulatorFuserightDao.GetAll(dbClient);

            if (table == null)
            {
                return;
            }

            foreach (DataRow dataRow in table.Rows)
            {
                this._rights.Add((string)dataRow["fuse"], Convert.ToInt32(dataRow["rank"]));
            }
        }
        public bool RankHasRight(int RankId, string Fuse)
        {
            if (!this._rights.ContainsKey(Fuse))
            {
                return false;
            }

            return RankId >= this._rights[Fuse];
        }

        public bool HasFuse(int Rank, string Fuse)
        {
            if (ButterflyEnvironment.GetGame().GetPermissionManager().RankHasRight(Rank, Fuse))
            {
                return true;
            }

            return false;
        }
    }
}