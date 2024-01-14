namespace WibboEmulator.Database.Daos.Guild;
using System.Data;
using Dapper;

internal sealed class GuildItemDao
{
    internal static List<GuildItemEntity> GetAll(IDbConnection dbClient) => dbClient.Query<GuildItemEntity>(
        "SELECT `id`, `type`, `firstvalue`, `secondvalue` FROM `guild_item` WHERE `enabled` = '1'"
    ).ToList();
}

public class GuildItemEntity
{
    public int Id { get; set; }
    public GuildItemType Type { get; set; }
    public string FirstValue { get; set; }
    public string SecondValue { get; set; }
    public bool Enabled { get; set; }
}

public enum GuildItemType
{
    Base,
    Symbol,
    Color,
    Color2,
    Color3
}