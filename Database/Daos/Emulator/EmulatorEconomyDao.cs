namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using Dapper;

internal sealed class EmulatorEconomyDao
{
    internal static List<EmulatorEconomyEntity> GetAll(IDbConnection dbClient) => dbClient.Query<EmulatorEconomyEntity>(
        "SELECT `id`, `category_id`, `item_id`, `extra_data`, `average_price` FROM `emulator_economy`"
    ).ToList();

    internal static EmulatorEconomyEntity GetById(IDbConnection dbClient, int id) => dbClient.QueryFirstOrDefault<EmulatorEconomyEntity>(
        "SELECT `id`, `category_id`, `item_id`, `extra_data`, `average_price` FROM `emulator_economy` WHERE `id` = @Id",
        new { Id = id }
    );

    internal static void Insert(IDbConnection dbClient, EmulatorEconomyEntity entity) => dbClient.Execute(
        "INSERT INTO `emulator_economy` (`category_id`, `item_id`, `extra_data`, `average_price`) VALUES (@CategoryId, @ItemId, @ExtraData, @AveragePrice)",
        entity);

    internal static void Update(IDbConnection dbClient, EmulatorEconomyEntity entity) => dbClient.Execute(
        "UPDATE `emulator_economy` SET `category_id` = @CategoryId, `item_id` = @ItemId, `extra_data` = @ExtraData, `average_price` = @AveragePrice WHERE `id` = @Id", entity);

    internal static void Delete(IDbConnection dbClient, int id) => dbClient.Execute(
        "DELETE FROM `emulator_economy` WHERE `id` = @Id",
        new { Id = id });
}

public class EmulatorEconomyEntity
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public int ItemId { get; set; }
    public string ExtraData { get; set; }
    public int AveragePrice { get; set; }
}
