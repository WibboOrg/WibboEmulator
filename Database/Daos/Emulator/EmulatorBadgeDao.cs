namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using Dapper;

internal sealed class EmulatorBadgeDao
{
    internal static List<EmulatorBadgeEntity> GetAll(IDbConnection dbClient) => dbClient.Query<EmulatorBadgeEntity>(
        "SELECT id, code, can_trade, can_delete, can_give, amount_winwins FROM `emulator_badge`"
    ).ToList();
}

public class EmulatorBadgeEntity
{
    public int Id { get; set; }
    public string Code { get; set; }
    public bool CanTrade { get; set; }
    public bool CanDelete { get; set; }
    public bool CanGive { get; set; }
    public int AmountWinwins { get; set; }
}
