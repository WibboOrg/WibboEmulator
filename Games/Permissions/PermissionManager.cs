namespace WibboEmulator.Games.Permissions;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;

public static class PermissionManager
{
    private static readonly Dictionary<string, int> Rights = new();
    private static readonly Dictionary<string, PermissionCommand> Commands = new();

    public static void Initialize(IDbConnection dbClient)
    {
        Rights.Clear();
        Commands.Clear();

        var emulatorPermissionList = EmulatorPermissionDao.GetAll(dbClient);

        foreach (var emulatorPermission in emulatorPermissionList)
        {
            Rights.Add(emulatorPermission.Permission, emulatorPermission.Rank);
        }
    }

    public static bool RankHasRight(int rankId, string fuse)
    {
        if (!Rights.TryGetValue(fuse, out var minRank))
        {
            return false;
        }

        return rankId >= minRank;
    }
}
