using Wibbo.Database.Interfaces;
using System.Data;

namespace Wibbo.Database.Daos
{
    class EmulatorHotelviewPromoDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT `index`, `header`, `body`, `button`, `in_game_promo`, `special_action`, `image`, `enabled` from `emulator_landingview` WHERE `enabled` = '1' ORDER BY `index` ASC");
            return dbClient.GetTable();
        }
    }
}