namespace WibboEmulator.Database.Daos.Catalog;
using System.Data;
using Dapper;

internal sealed class CatalogPromotionDao
{
    internal static List<CatalogPromotionEntity> GetAll(IDbConnection dbClient) => dbClient.Query<CatalogPromotionEntity>(
        "SELECT `id`, `title`, `title_en`, `title_br`, `image`, `unknown`, `page_link`, `parent_id` FROM `catalog_promotion`"
    ).ToList();
}

public class CatalogPromotionEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string TitleEn { get; set; }
    public string TitleBr { get; set; }
    public string Image { get; set; }
    public int Unknown { get; set; }
    public string PageLink { get; set; }
    public int ParentId { get; set; }
}
