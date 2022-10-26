namespace WibboEmulator.Games.Catalogs.Vouchers;
using System.Data;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Database.Interfaces;

public class VoucherManager
{
    private readonly Dictionary<string, Voucher> _vouchers;

    public VoucherManager() => this._vouchers = new Dictionary<string, Voucher>();

    public void Init(IQueryAdapter dbClient)
    {
        if (this._vouchers.Count > 0)
        {
            this._vouchers.Clear();
        }

        var getVouchers = CatalogVoucherDao.GetAll(dbClient);

        if (getVouchers != null)
        {
            foreach (DataRow row in getVouchers.Rows)
            {
                this._vouchers.Add(Convert.ToString(row["voucher"]), new Voucher(Convert.ToString(row["voucher"]), Convert.ToString(row["type"]), Convert.ToInt32(row["value"]), Convert.ToInt32(row["current_uses"]), Convert.ToInt32(row["max_uses"])));
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
