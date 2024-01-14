namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using Dapper;

public class EmulatorLootBoxDao
{
    internal static List<EmulatorLootboxEntity> GetAll(IDbConnection dbClient) => dbClient.Query<EmulatorLootboxEntity>(
        "SELECT `interaction_type`, `probability`, `page_id`, `item_id`, `category`, `amount` FROM `emulator_lootbox`"
    ).ToList();
}

public class EmulatorLootboxEntity
{
    public int Id { get; set; }
    public string InteractionType { get; set; }
    public int Probability { get; set; }
    public int PageId { get; set; }
    public int ItemId { get; set; }
    public LootboxCategory Category { get; set; }
    public int Amount { get; set; }
}

public enum LootboxCategory
{
    Furni,
    Badge,
    WinWin,
    Credits
}