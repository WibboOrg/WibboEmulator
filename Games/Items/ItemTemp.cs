namespace WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms.Map.Movement;

public class ItemTemp : IEquatable<ItemTemp>
{
    public int Id { get; set; }
    public int VirtualUserId { get; set; }
    public int SpriteId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public double Z { get; set; }
    public MovementDirection Movement { get; set; }
    public string ExtraData { get; set; }
    public int Value { get; set; }
    public int TeamId { get; set; }
    public int Distance { get; set; }
    public InteractionTypeTemp InteractionType { get; set; }

    public ItemTemp(int id, int userId, int spriteId, int x, int y, double z, string extraData, MovementDirection movement, int value, InteractionTypeTemp interaction, int distance = 0, int teamId = 0)
    {
        this.Id = id;
        this.VirtualUserId = userId;
        this.SpriteId = spriteId;
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.ExtraData = extraData;
        this.Movement = movement;
        this.Value = value;
        this.TeamId = teamId;
        this.Distance = distance;
        this.InteractionType = interaction;
    }

    public bool Equals(ItemTemp other) => other.Id == this.Id;

    public override bool Equals(object obj) => this.Equals(obj as ItemTemp);

    public override int GetHashCode() => this.Id;
}

public enum InteractionTypeTemp
{
    None,
    Projectile,
    ProjectileBot,
    Grenade,
    Money,
    RpItem
}
