namespace WibboEmulator.Database.Daos.Catalog;
using System.Data;
using Dapper;

internal sealed class CatalogVoucherDao
{
    internal static void Update(IDbConnection dbClient, string code) => dbClient.Execute(
        "UPDATE catalog_voucher SET current_uses = current_uses + 1 WHERE voucher = @Code LIMIT 1",
        new { Code = code });

    internal static List<CatalogVoucherEntity> GetAll(IDbConnection dbClient) => dbClient.Query<CatalogVoucherEntity>(
        "SELECT `voucher`, `type`, `value`, `current_uses`, `max_uses` FROM `catalog_voucher` WHERE `enabled` = '1'"
    ).ToList();
}

public class CatalogVoucherEntity
{
    public string Voucher { get; set; }
    public VoucherType Type { get; set; }
    public int Value { get; set; }
    public int CurrentUses { get; set; }
    public int MaxUses { get; set; }
    public bool Enabled { get; set; }
}

public enum VoucherType
{
    Credits,
    Duckets
}
