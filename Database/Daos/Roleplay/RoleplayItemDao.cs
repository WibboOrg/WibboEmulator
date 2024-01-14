namespace WibboEmulator.Database.Daos.Roleplay;
using System.Data;
using Dapper;

internal sealed class RoleplayItemDao
{
    internal static List<RoleplayItemEntity> GetAll(IDbConnection dbClient) => dbClient.Query<RoleplayItemEntity>(
        "SELECT `id`, `name`, `desc`, `price`, `type`, `value`, `allowstack`, `category` FROM `roleplay_item`"
    ).ToList();
}

public class RoleplayItemEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Desc { get; set; }
    public int Price { get; set; }
    public string Type { get; set; }
    public int Value { get; set; }
    public bool AllowStack { get; set; }
    public string Category { get; set; }
}
