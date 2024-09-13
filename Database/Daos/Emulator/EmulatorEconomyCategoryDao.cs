namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using Dapper;

internal sealed class EmulatorEconomyCategoryDao
{
    internal static List<EmulatorEconomyCategoryEntity> GetAll(IDbConnection dbClient) => dbClient.Query<EmulatorEconomyCategoryEntity>(
        "SELECT `id`, `parent_id`, `icon_image`, `caption` FROM `emulator_economy_category` ORDER BY `order_num` ASC"
    ).ToList();
}

public class EmulatorEconomyCategoryEntity
{
    public int Id { get; set; }
    public int ParentId { get; set; }
    public int IconImage { get; set; }
    public string Caption { get; set; }
}
