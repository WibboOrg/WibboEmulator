namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using Dapper;

internal sealed class EmulatorPermissionDao
{
    internal static List<EmulatorPermissionEntity> GetAll(IDbConnection dbClient) => dbClient.Query<EmulatorPermissionEntity>(
        "SELECT `rank`, `permission` FROM `emulator_permission`"
    ).ToList();
}

public class EmulatorPermissionEntity
{
    public int Id { get; set; }
    public int Rank { get; set; }
    public string Permission { get; set; }
}