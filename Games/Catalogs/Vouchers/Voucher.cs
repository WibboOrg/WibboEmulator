namespace WibboEmulator.Games.Catalogs.Vouchers;

using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Catalog;

public class Voucher(string code, string type, int value, int currentUses, int maxUses)
{
    public void UpdateUses()
    {
        this.CurrentUses += 1;

        using var dbClient = DatabaseManager.Connection;
        CatalogVoucherDao.Update(dbClient, this.Code);
    }

    public string Code { get; set; } = code;
    public VoucherType Type { get; set; } = VoucherUtility.GetType(type);
    public int Value { get; set; } = value;
    public int CurrentUses { get; set; } = currentUses;
    public int MaxUses { get; set; } = maxUses;
}
