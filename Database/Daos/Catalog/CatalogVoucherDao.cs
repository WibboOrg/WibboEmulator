namespace WibboEmulator.Database.Daos.Catalog;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal sealed class CatalogVoucherDao
{
    internal static void Update(IQueryAdapter dbClient, string code)
    {
        dbClient.SetQuery("UPDATE `catalog_voucher` SET `current_uses` = `current_uses` + '1' WHERE `voucher` = @code LIMIT 1");
        dbClient.AddParameter("code", code);
        dbClient.RunQuery();
    }

    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT `voucher`, `type`, `value`, `current_uses`, `max_uses` FROM `catalog_voucher` WHERE `enabled` = '1'");
        return dbClient.GetTable();
    }
}