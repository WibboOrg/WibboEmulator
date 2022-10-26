namespace WibboEmulator.Games.Catalogs.Vouchers;

public static class VoucherUtility
{
    public static VoucherType GetType(string type) => type switch
    {
        "ducket" => VoucherType.Ducket,
        "badge" => VoucherType.Badge,
        "winwin" => VoucherType.Winwin,
        "wibbopoints" => VoucherType.WibboPoint,
        "jetons" => VoucherType.Jeton,
        _ => VoucherType.Credit,
    };

    public static string FromType(VoucherType type) => type switch
    {
        VoucherType.Ducket => "ducket",
        VoucherType.Badge => "badge",
        VoucherType.Winwin => "winwin",
        VoucherType.WibboPoint => "wibbopoints",
        VoucherType.Jeton => "jetons",
        _ => "credit",
    };
}
