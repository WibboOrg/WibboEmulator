namespace WibboEmulator.Games.Catalogs.Vouchers;

using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Catalog;

public class Voucher
{
    public Voucher(string code, string type, int value, int currentUses, int maxUses)
    {
        this.Code = code;
        this.Type = VoucherUtility.GetType(type);
        this.Value = value;
        this.CurrentUses = currentUses;
        this.MaxUses = maxUses;
    }

    public void UpdateUses()
    {
        this.CurrentUses += 1;

        using var dbClient = DatabaseManager.Connection;
        CatalogVoucherDao.Update(dbClient, this.Code);
    }

    public string Code { get; set; }
    public VoucherType Type { get; set; }
    public int Value { get; set; }
    public int CurrentUses { get; set; }
    public int MaxUses { get; set; }
}
