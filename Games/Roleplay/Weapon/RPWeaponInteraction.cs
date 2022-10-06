namespace WibboEmulator.Games.Roleplay.Weapon;

public enum RPWeaponInteraction
{
    NONE,
}
public class RPWeaponInteractions
{
    public static RPWeaponInteraction GetTypeFromString(string pType) => pType switch
    {
        "none" => RPWeaponInteraction.NONE,
        _ => RPWeaponInteraction.NONE,
    };
}
