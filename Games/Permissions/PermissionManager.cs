namespace WibboEmulator.Games.Permissions;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;

public class PermissionManager
{
    private readonly Dictionary<string, int> _rights;
    private readonly Dictionary<string, PermissionCommand> _commands;

    public PermissionManager()
    {
        this._rights = new Dictionary<string, int>();
        this._commands = new Dictionary<string, PermissionCommand>();
    }

    public void Init(IDbConnection dbClient)
    {
        this._rights.Clear();
        this._commands.Clear();

        var emulatorPermissionList = EmulatorPermissionDao.GetAll(dbClient);

        if (emulatorPermissionList.Count == 0)
        {
            return;
        }

        foreach (var emulatorPermission in emulatorPermissionList)
        {
            this._rights.Add(emulatorPermission.Permission, emulatorPermission.Rank);
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
