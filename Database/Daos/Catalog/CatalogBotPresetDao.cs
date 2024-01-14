namespace WibboEmulator.Database.Daos.Catalog;
using System.Data;
using Dapper;

internal sealed class CatalogBotPresetDao
{
    internal static List<CatalogBotPresetEntity> GetAll(IDbConnection dbClient) => dbClient.Query<CatalogBotPresetEntity>(
        "SELECT `id`, `name`, `figure`, `motto`, `gender`, `ai_type` FROM `catalog_bot_preset`"
    ).ToList();
}

public class CatalogBotPresetEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Figure { get; set; }
    public string Gender { get; set; }
    public string Motto { get; set; }
    public string AiType { get; set; }
}