namespace WibboEmulator.Games.Roleplays.Weapon;

public class RPWeapon(int id, int dmgMin, int dmgMax, RPWeaponInteraction interaction, int enable, int freezeTime, int distance)
{
    public int Id { get; set; } = id;
    public int DmgMin { get; set; } = dmgMin;
    public int DmgMax { get; set; } = dmgMax;
    public RPWeaponInteraction Interaction { get; set; } = interaction;
    public int Enable { get; set; } = enable;
    public int FreezeTime { get; set; } = freezeTime;
    public int Distance { get; set; } = distance;
}
