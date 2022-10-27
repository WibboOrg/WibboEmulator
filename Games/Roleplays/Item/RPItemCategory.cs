namespace WibboEmulator.Games.Roleplays.Item;

public enum RPItemCategory
{
    Equip,
    Util,
    Ressouce,
    Quete,
}
public class RPItemCategorys
{
    public static RPItemCategory GetTypeFromString(string pType) => pType switch
    {
        "EQUIP" => RPItemCategory.Equip,
        "UTIL" => RPItemCategory.Util,
        "RESSOURCE" => RPItemCategory.Ressouce,
        "QUETE" => RPItemCategory.Quete,
        _ => RPItemCategory.Quete,
    };
}
