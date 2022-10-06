namespace WibboEmulator.Games.Catalog.Vouchers;

public static class VoucherUtility
{
    public static VoucherType GetType(string type) => type switch
    {
        "ducket" => VoucherType.DUCKET,
        "badge" => VoucherType.BADGE,
        "winwin" => VoucherType.WINWIN,
        "wibbopoints" => VoucherType.WIBBOPOINTS,
        "jetons" => VoucherType.JETONS,
        _ => VoucherType.CREDIT,
    };

    public static string FromType(VoucherType type) => type switch
    {
        VoucherType.DUCKET => "ducket",
        VoucherType.BADGE => "badge",
        VoucherType.WINWIN => "winwin",
        VoucherType.WIBBOPOINTS => "wibbopoints",
        VoucherType.JETONS => "jetons",
        _ => "credit",
    };
}
