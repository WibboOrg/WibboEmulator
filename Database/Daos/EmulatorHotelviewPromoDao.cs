using System.Data;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class EmulatorHotelviewPromoDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * from hotelview_promos WHERE enabled = '1' ORDER BY index ASC");
            return dbClient.GetTable();
        }
    }
}