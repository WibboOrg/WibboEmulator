using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class CatalogVoucherDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE catalog_vouchers SET current_uses = current_uses + '1' WHERE voucher = '" + this._code + "' LIMIT 1");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT voucher,type,value,current_uses,max_uses FROM catalog_vouchers WHERE enabled = '1'");
            GetVouchers = dbClient.GetTable();
        }
    }
}