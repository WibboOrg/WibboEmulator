namespace WibboEmulator.Database.Daos.Navigator;
using System.Data;
using Dapper;

internal sealed class NavigatorPublicDao
{
    internal static List<NavigatorPublicEntity> GetAll(IDbConnection dbClient) => dbClient.Query<NavigatorPublicEntity>(
        "SELECT room_id, image_url, langue, category_type FROM `navigator_public` WHERE enabled = '1' ORDER BY order_num ASC"
    ).ToList();
}

public class NavigatorPublicEntity
{
    public int RoomId { get; set; }
    public string ImageUrl { get; set; }
    public int OrderNum { get; set; }
    public bool Enabled { get; set; }
    public string Langue { get; set; }
    public string CategoryType { get; set; }
}