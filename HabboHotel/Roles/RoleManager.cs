using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.HabboHotel.Roles
{
    public class RoleManager
    {
        private readonly Dictionary<string, int> Rights;

        public RoleManager()
        {
            this.Rights = new Dictionary<string, int>();
        }

        public void Init()
        {
            this.Rights.Clear();

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
    }
}
