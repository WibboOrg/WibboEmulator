namespace WibboEmulator.Games.Roleplay.Weapon;

public class RPWeapon
{
    public int Id { get; set; }
    public int DmgMin { get; set; }
    public int DmgMax { get; set; }
    public RPWeaponInteraction Interaction { get; set; }
    public int Enable { get; set; }
    public int FreezeTime { get; set; }
    public int Distance { get; set; }

    public RPWeapon(int id, int dmgMin, int dmgMax, RPWeaponInteraction interaction, int enable, int freezeTime, int distance)
    {
        this.Id = id;
        this.DmgMin = dmgMin;
        this.DmgMax = dmgMax;
        this.Interaction = interaction;
        this.Enable = enable;
        this.FreezeTime = freezeTime;
        this.Distance = distance;
    }
}
