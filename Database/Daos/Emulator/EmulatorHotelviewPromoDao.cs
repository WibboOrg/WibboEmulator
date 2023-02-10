namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal sealed class EmulatorHotelviewPromoDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT `index`, `header`, `body`, `button`, `in_game_promo`, `special_action`, `image`, `enabled` from `emulator_landingview` WHERE `enabled` = '1' ORDER BY `index` ASC");
        return dbClient.GetTable();
    }
}