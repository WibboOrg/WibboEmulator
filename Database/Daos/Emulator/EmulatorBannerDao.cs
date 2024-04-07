namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using Dapper;

internal sealed class EmulatorBannerDao
{
    internal static List<EmulatorBannerEntity> GetAll(IDbConnection dbClient) => dbClient.Query<EmulatorBannerEntity>(
        "SELECT `id`, `have_layer`, `can_trade` FROM `emulator_banner`"
    ).ToList();
}

public class EmulatorBannerEntity
{
    public int Id { get; set; }
    public bool HaveLayer { get; set; }
    public bool CanTrade { get; set; }
}
