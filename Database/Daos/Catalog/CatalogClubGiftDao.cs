namespace WibboEmulator.Database.Daos.Catalog;

using System.Data;
using WibboEmulator.Database.Interfaces;

internal sealed class CatalogClubGiftDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT catalog_item_id, days_required FROM `catalog_item`");

        return dbClient.GetTable();
    }
}
