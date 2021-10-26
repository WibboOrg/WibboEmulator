using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class EmulatorHotelviewPromoDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            DbClient.SetQuery("SELECT * from hotelview_promos WHERE enabled = '1' ORDER BY index ASC");
            DataTable dTable = DbClient.GetTable();
        }
    }
}