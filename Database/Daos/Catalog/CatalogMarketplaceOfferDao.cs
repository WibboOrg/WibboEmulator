namespace WibboEmulator.Database.Daos.Catalog;
using System.Data;
using System.Text;
using WibboEmulator.Games.Catalogs.Marketplace;
using Dapper;

internal sealed class CatalogMarketplaceOfferDao
{
    internal static CatalogMarketplaceOfferEntity GetOneByOfferId(IDbConnection dbClient, int offerId) => dbClient.QuerySingleOrDefault<CatalogMarketplaceOfferEntity>(
        @"SELECT state, timestamp, total_price, extra_data, item_id, furni_id, user_id, limited_number, limited_stack 
        FROM `catalog_marketplace_offer` 
        WHERE offer_id = @OfferId 
        LIMIT 1",
        new { OfferId = offerId });

    internal static void UpdateState(IDbConnection dbClient, int offerId) => dbClient.Execute(
        "UPDATE `catalog_marketplace_offer` SET state = '2' WHERE offer_id = '" + offerId + "' LIMIT 1");

    internal static List<CatalogMarketplaceOfferEntity> GetAll(IDbConnection dbConnection, string searchQuery, int minCost, int maxCost, int filterMode)
    {
        var builder = new StringBuilder();
        _ = builder.Append("WHERE state = '1' AND timestamp >= " + MarketplaceManager.FormatTimestamp());

        if (minCost >= 0)
        {
            _ = builder.Append(" AND total_price > " + minCost);
        }

        if (maxCost >= 0)
        {
            _ = builder.Append(" AND total_price < " + maxCost);
        }

        var str = filterMode switch
        {
            1 => "ORDER BY asking_price DESC",
            _ => "ORDER BY asking_price ASC",
        };

        if (searchQuery.Length >= 1)
        {
            _ = builder.Append(" AND public_name LIKE @SearchQuery");
        }

        return dbConnection.Query<CatalogMarketplaceOfferEntity>(
            @"SELECT offer_id, item_type, sprite_id, total_price, limited_number, limited_stack 
            FROM catalog_marketplace_offer 
            " + builder + " " + str + " LIMIT 500",
            new { SearchQuery = searchQuery.Replace("%", "\\%").Replace("_", "\\_") + "%" }
        ).ToList();
    }

    internal static void DeleteUserOffer(IDbConnection dbConnection, int offerId, int userId) => dbConnection.Execute(
        "DELETE FROM catalog_marketplace_offer WHERE offer_id = @OfferId AND user_id = @UserId LIMIT 1",
        new { OfferId = offerId, UserId = userId });

    internal static CatalogMarketplaceOfferEntity GetByOfferId(IDbConnection dbClient, int offerId) => dbClient.QuerySingleOrDefault<CatalogMarketplaceOfferEntity>(
        @"SELECT furni_id, item_id, user_id, extra_data, offer_id, state, timestamp, limited_number, limited_stack 
        FROM `catalog_marketplace_offer` 
        WHERE offer_id = @OfferId 
        LIMIT 1",
        new { OfferId = offerId });

    internal static void Insert(IDbConnection dbConnection, string itemName, string extraData, int itemId, int baseItem, int userId, int sellingPrice, int totalPrice, int spriteId, int itemType, int limited, int limitedStack) => dbConnection.Execute(
        @"INSERT INTO catalog_marketplace_offer 
        (furni_id, item_id, user_id, asking_price, total_price, public_name, sprite_id, item_type, timestamp, extra_data, limited_number, limited_stack) 
        VALUES (@ItemId, @BaseItem, @UserId, @SellingPrice, @TotalPrice, @PublicName, @SpriteId, @ItemType, @Timestamp, @ExtraData, @Limited, @LimitedStack)",
        new
        {
            ItemId = itemId,
            BaseItem = baseItem,
            UserId = userId,
            SellingPrice = sellingPrice,
            TotalPrice = totalPrice,
            PublicName = itemName,
            SpriteId = spriteId,
            ItemType = itemType,
            Timestamp = WibboEnvironment.GetUnixTimestamp(),
            ExtraData = extraData,
            Limited = limited,
            LimitedStack = limitedStack
        });

    internal static void Delete(IDbConnection dbClient, int userId) => dbClient.Execute(
        "DELETE FROM `catalog_marketplace_offer` WHERE user_id = '" + userId + "' AND state = '2'");

    internal static List<CatalogMarketplaceOfferEntity> GetAllByUserId(IDbConnection dbClient, int userId) => dbClient.Query<CatalogMarketplaceOfferEntity>(
        @"SELECT timestamp, state, offer_id, item_type, sprite_id, total_price, limited_number, limited_stack 
        FROM `catalog_marketplace_offer` 
        WHERE user_id = '" + userId + "'"
    ).ToList();

    internal static int GetSunPrice(IDbConnection dbClient, int userId) => dbClient.ExecuteScalar<int>(
        "SELECT SUM(asking_price) FROM `catalog_marketplace_offer` WHERE state = '2' AND user_id = '" + userId + "'");

    internal static int GetOneLTD(IDbConnection dbClient, int itemId, int limitedNumber) => dbClient.ExecuteScalar<int>(
        "SELECT offer_id FROM `catalog_marketplace_offer` WHERE item_id = '" + itemId + "' AND limited_number = '" + limitedNumber + "'");
}

public class CatalogMarketplaceOfferEntity
{
    public int OfferId { get; set; }
    public int UserId { get; set; }
    public int ItemId { get; set; }
    public int AskingPrice { get; set; }
    public int TotalPrice { get; set; }
    public string PublicName { get; set; }
    public int SpriteId { get; set; }
    public int ItemType { get; set; }
    public int Timestamp { get; set; }
    public int State { get; set; }
    public string ExtraData { get; set; }
    public int FurniId { get; set; }
    public int LimitedNumber { get; set; }
    public int LimitedStack { get; set; }
}
