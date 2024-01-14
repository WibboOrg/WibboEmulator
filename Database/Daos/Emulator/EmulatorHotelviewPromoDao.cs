namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using Dapper;

internal sealed class EmulatorHotelviewPromoDao
{
    internal static List<EmulatorLandingViewEntity> GetAll(IDbConnection dbClient) => dbClient.Query<EmulatorLandingViewEntity>(
        @"SELECT `index`, `header`, `body`, `button`, `in_game_promo`, `special_action`, `image`, `enabled` 
        FROM `emulator_landingview` 
        WHERE `enabled` = '1'
        ORDER BY `index` ASC"
    ).ToList();
}

public class EmulatorLandingViewEntity
{
    public int Index { get; set; }
    public string Header { get; set; }
    public string Body { get; set; }
    public string Button { get; set; }
    public bool InGamePromo { get; set; }
    public string SpecialAction { get; set; }
    public string Image { get; set; }
    public bool Enabled { get; set; }
}
