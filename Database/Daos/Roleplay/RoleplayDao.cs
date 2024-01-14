namespace WibboEmulator.Database.Daos.Roleplay;

using System.Data;
using Dapper;

internal sealed class RoleplayDao
{
    internal static List<RoleplayEntity> GetAll(IDbConnection dbClient) => dbClient.Query<RoleplayEntity>(
        "SELECT owner_id, hopital_id, prison_id FROM `roleplay`"
    ).ToList();
}

public class RoleplayEntity
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public int HopitalId { get; set; }
    public int PrisonId { get; set; }
}