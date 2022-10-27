namespace WibboEmulator.Games.Roleplays.Enemy;

public class RPEnemy
{
    public int Id { get; set; }
    public int Health { get; set; }
    public int WeaponGunId { get; set; }
    public int WeaponCacId { get; set; }
    public int DeadTimer { get; set; }
    public int MoneyDrop { get; set; }
    public int DropScriptId { get; set; }
    public int TeamId { get; set; }
    public int ZoneDistance { get; set; }
    public bool ResetPosition { get; set; }
    public int AggroDistance { get; set; }
    public int LootItemId { get; set; }
    public int LostAggroDistance { get; set; }
    public bool ZombieMode { get; set; }

    public RPEnemy(int id, int health, int weaponGunId, int weaponCacId, int deadTimer, int lootItemId, int moneyDrop, int dropScriptId, int teamId, int aggroDistance, int zoneDistance,
        bool resetPosition, int lostAggroDistance, bool zombieMode)
    {
        this.Id = id;
        this.Health = health;
        this.WeaponGunId = weaponGunId;
        this.WeaponCacId = weaponCacId;
        this.DeadTimer = deadTimer;
        this.MoneyDrop = moneyDrop;
        this.LootItemId = lootItemId;
        this.MoneyDrop = moneyDrop;
        this.DropScriptId = dropScriptId;
        this.TeamId = teamId;
        this.AggroDistance = aggroDistance;
        this.ZoneDistance = zoneDistance;
        this.ResetPosition = resetPosition;
        this.LostAggroDistance = lostAggroDistance;
        this.ZombieMode = zombieMode;
    }
}
