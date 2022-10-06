namespace WibboEmulator.Games.Roleplay.Item;

public enum RPItemCategory
{
    EQUIP,
    UTIL,
    RESSOURCE,
    QUETE,
}
public class RPItemCategorys
{
    public static RPItemCategory GetTypeFromString(string pType) => pType switch
    {
        "EQUIP" => RPItemCategory.EQUIP,
        "UTIL" => RPItemCategory.UTIL,
        "RESSOURCE" => RPItemCategory.RESSOURCE,
        "QUETE" => RPItemCategory.QUETE,
        _ => RPItemCategory.QUETE,
    };
}
