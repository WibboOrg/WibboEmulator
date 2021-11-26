using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Permissions
{
    public class PermissionManager
    {
        private readonly Dictionary<string, int> Rights;

        private readonly Dictionary<string, PermissionCommand> _commands = new Dictionary<string, PermissionCommand>();

        public PermissionManager()
        {
            this.Rights = new Dictionary<string, int>();
        }

        public void Init()
        {
            this.Rights.Clear();
            _commands.Clear();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable table = EmulatorFuserightDao.GetAll(dbClient);

                if (table == null)
                {
                    return;
                }

                foreach (DataRow dataRow in table.Rows)
                {
                    this.Rights.Add((string)dataRow["fuse"], Convert.ToInt32(dataRow["rank"]));
                }
            }
        }
        public bool RankHasRight(int RankId, string Fuse)
        {
            if (!this.Rights.ContainsKey(Fuse))
            {
                return false;
            }

            return RankId >= this.Rights[Fuse];
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