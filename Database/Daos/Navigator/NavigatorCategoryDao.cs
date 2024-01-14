namespace WibboEmulator.Database.Daos.Navigator;
using System.Data;
using Dapper;

internal sealed class NavigatorCategoryDao
{
    internal static List<NavigatorCategoryEntity> GetAll(IDbConnection dbClient) => dbClient.Query<NavigatorCategoryEntity>(
        "SELECT `id`, `category`, `category_identifier`, `public_name`, `view_mode`, `required_rank`, `category_type`, `search_allowance`, `minimized`, `order_id` FROM `navigator_category` WHERE `enabled` = '1' ORDER BY `id` ASC"
    ).ToList();
}

public class NavigatorCategoryEntity
{
    public int Id { get; set; }
    public string Category { get; set; }
    public string CategoryIdentifier { get; set; }
    public string PublicName { get; set; }
    public string ViewMode { get; set; }
    public int RequiredRank { get; set; }
    public string CategoryType { get; set; }
    public string SearchAllowance { get; set; }
    public bool Minimized { get; set; }
    public bool Enabled { get; set; }
    public int OrderId { get; set; }
}