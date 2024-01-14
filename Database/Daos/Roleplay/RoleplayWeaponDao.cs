namespace WibboEmulator.Database.Daos.Roleplay;
using System.Data;
using Dapper;

internal sealed class RoleplayWeaponDao
{
    internal static List<RoleplayWeaponEntity> GetAll(IDbConnection dbClient) => dbClient.Query<RoleplayWeaponEntity>(
        "SELECT `id`, `type`, `domage_min`, `domage_max`, `interaction`, `enable`, `freeze_time`, `distance` FROM `roleplay_weapon`"
    ).ToList();
}

public class RoleplayWeaponEntity
{
    public int Id { get; set; }
    public string Type { get; set; }
    public int DomageMin { get; set; }
    public int DomageMax { get; set; }
    public string Interaction { get; set; }
    public int Enable { get; set; }
    public int FreezeTime { get; set; }
    public int Distance { get; set; }
}