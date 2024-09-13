namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using Dapper;

internal sealed class EmulatorEconomyDao
{
    internal static List<EmulatorEconomyEntity> GetAll(IDbConnection dbClient) => dbClient.Query<EmulatorEconomyEntity>(
        "SELECT `id`, `category_id`, `item_id`, `extra_data`, `average_price` FROM `emulator_economy`"
    ).ToList();
}

public class EmulatorEconomyEntity
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public int ItemId { get; set; }
    public string ExtraData { get; set; }
    public int AveragePrice { get; set; }
}
