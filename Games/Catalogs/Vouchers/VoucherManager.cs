namespace WibboEmulator.Games.Catalogs.Vouchers;
using System.Data;
using WibboEmulator.Database.Daos.Catalog;

public static class VoucherManager
{
    private static readonly Dictionary<string, Voucher> Vouchers = new();

    public static void Initialize(IDbConnection dbClient)
    {
        if (Vouchers.Count > 0)
        {
            Vouchers.Clear();
        }

        var voucherList = CatalogVoucherDao.GetAll(dbClient);

        if (voucherList.Count != 0)
        {
            foreach (var voucher in voucherList)
            {
                Vouchers.Add(voucher.Voucher, new Voucher(voucher.Voucher, voucher.Type.ToString(), voucher.Value, voucher.CurrentUses, voucher.MaxUses));
            }
        }
    }

    public static bool TryGetVoucher(string code, out Voucher voucher)
    {
        if (Vouchers.TryGetValue(code, out voucher))
        {
            return true;
        }

        return false;
    }
}
