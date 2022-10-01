using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using System.Data;

namespace WibboEmulator.Games.Permissions
{
    public class PermissionManager
    {
        private readonly Dictionary<string, int> _rights;
        private readonly Dictionary<string, PermissionCommand> _commands;

        public PermissionManager()
        {
            this._rights = new Dictionary<string, int>();
            this._commands = new Dictionary<string, PermissionCommand>();
        }

        public void Init(IQueryAdapter dbClient)
        {
            this._rights.Clear();
            this._commands.Clear();

            DataTable table = EmulatorPermissionDao.GetAll(dbClient);

            if (table == null)
            {
                return;
            }

            foreach (DataRow dataRow in table.Rows)
            {
                this._rights.Add((string)dataRow["permission"], Convert.ToInt32(dataRow["rank"]));
            }
        }

        public bool RankExactRight(int RankId, string Fuse)
        {
            if (!this._rights.ContainsKey(Fuse))
            {
                return false;
            }

            return RankId == this._rights[Fuse];
        }

        public bool RankHasRight(int RankId, string Fuse)
        {
            if (!this._rights.ContainsKey(Fuse))
            {
                return false;
            }

            return RankId >= this._rights[Fuse];
        }
    }
}