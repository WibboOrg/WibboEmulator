namespace WibboEmulator.Database.Daos.Catalog;
using System.Data;
using Dapper;

internal sealed class CatalogItemDao
{
    internal static List<CatalogItemEntity> GetAll(IDbConnection dbClient) => dbClient.Query<CatalogItemEntity>(
        @"SELECT id, item_id, catalog_name, cost_credits, cost_pixels, cost_diamonds, cost_limitcoins, amount, page_id, catalog_item_limited.limited_sells, catalog_item_limited.limited_stack, offer_active, badge 
        FROM `catalog_item` 
        LEFT JOIN `catalog_item_limited` 
        ON (`catalog_item_limited`.catalog_item_id = id) 
        ORDER by ID DESC"
    ).ToList();
}

public class CatalogItemEntity
{
    public int Id { get; set; }
    public int PageId { get; set; }
    public int ItemId { get; set; }
    public string CatalogName { get; set; }
    public int CostCredits { get; set; }
    public int CostPixels { get; set; }
    public int CostDiamonds { get; set; }
    public int CostLimitCoins { get; set; }
    public int Amount { get; set; }
    public bool OfferActive { get; set; }
    public string Badge { get; set; }
    public int LimitedSells { get; set; }
    public int LimitedStack { get; set; }
}
