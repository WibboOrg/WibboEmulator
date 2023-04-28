namespace WibboEmulator.Games.Catalogs.Vouchers;

public static class VoucherUtility
{
    public static VoucherType GetType(string type) => type switch
    {
        "ducket" => VoucherType.Ducket,
        "badge" => VoucherType.Badge,
        "winwin" => VoucherType.Winwin,
        "wibbopoints" => VoucherType.WibboPoint,
        _ => VoucherType.Credit,
    };

    public static string FromType(VoucherType type) => type switch
    {
        VoucherType.Ducket => "ducket",
        VoucherType.Badge => "badge",
        VoucherType.Winwin => "winwin",
        VoucherType.WibboPoint => "wibbopoints",
        _ => "credit",
    };
}
