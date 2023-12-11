namespace WibboEmulator.Games.Permissions;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Interfaces;

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

        var table = EmulatorPermissionDao.GetAll(dbClient);

        if (table == null)
        {
            return;
        }

        foreach (DataRow dataRow in table.Rows)
        {
            this._rights.Add((string)dataRow["permission"], Convert.ToInt32(dataRow["rank"]));
        }
    }

    public bool RankHasRight(int rankId, string fuse)
    {
        if (!this._rights.TryGetValue(fuse, out var minRank))
        {
            return false;
        }

        return rankId >= minRank;
    }
}
