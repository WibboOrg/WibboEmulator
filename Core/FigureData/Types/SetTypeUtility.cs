namespace WibboEmulator.Core.FigureData.Types;

public static class SetTypeUtility
{
    public static SetType GetSetType(string type) => type switch
    {
        "HD" => SetType.HD,
        "CH" => SetType.CH,
        "LG" => SetType.LG,
        "SH" => SetType.SH,
        "HA" => SetType.HA,
        "HE" => SetType.HE,
        "EA" => SetType.EA,
        "FA" => SetType.FA,
        "CA" => SetType.CA,
        "WA" => SetType.WA,
        "CC" => SetType.CC,
        "CP" => SetType.CP,
        _ => SetType.HR,
    };
}
