namespace WibboEmulator.Games.Catalog.Vouchers;
using WibboEmulator.Database.Daos.Catalog;

public class Voucher
{
    public Voucher(string Code, string Type, int Value, int CurrentUses, int MaxUses)
    {
        this.Code = Code;
        this.Type = VoucherUtility.GetType(Type);
        this.Value = Value;
        this.CurrentUses = CurrentUses;
        this.MaxUses = MaxUses;
    }

    public void UpdateUses()
    {
        this.CurrentUses += 1;

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        CatalogVoucherDao.Update(dbClient, this.Code);
    }

    public string Code { get; set; }

    public VoucherType Type { get; set; }

    public int Value { get; set; }

    public int CurrentUses { get; set; }

    public int MaxUses { get; set; }
}
