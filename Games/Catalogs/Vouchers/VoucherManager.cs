namespace WibboEmulator.Games.Catalogs.Vouchers;
using System.Data;
using WibboEmulator.Database.Daos.Catalog;

public class VoucherManager
{
    private readonly Dictionary<string, Voucher> _vouchers;

    public VoucherManager() => this._vouchers = new Dictionary<string, Voucher>();

    public void Initialize(IDbConnection dbClient)
    {
        if (this._vouchers.Count > 0)
        {
            this._vouchers.Clear();
        }

        var voucherList = CatalogVoucherDao.GetAll(dbClient);

        if (voucherList.Count != 0)
        {
            foreach (var voucher in voucherList)
            {
                this._vouchers.Add(voucher.Voucher, new Voucher(voucher.Voucher, voucher.Type.ToString(), voucher.Value, voucher.CurrentUses, voucher.MaxUses));
            }
        }
    }

    public bool TryGetVoucher(string code, out Voucher voucher)
    {
        if (this._vouchers.TryGetValue(code, out voucher))
        {
            return true;
        }

        return false;
    }
}
