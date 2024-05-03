namespace WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms.Map.Movement;

public class ItemTemp(int id, int userId, int spriteId, int x, int y, double z, string extraData, MovementDirection movement, int value, InteractionTypeTemp interaction, int distance = 0, int teamId = 0) : IEquatable<ItemTemp>
{
    public int Id { get; set; } = id;
    public int VirtualUserId { get; set; } = userId;
    public int SpriteId { get; set; } = spriteId;
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
    public double Z { get; set; } = z;
    public MovementDirection Movement { get; set; } = movement;
    public string ExtraData { get; set; } = extraData;
    public int Value { get; set; } = value;
    public int TeamId { get; set; } = teamId;
    public int Distance { get; set; } = distance;
    public InteractionTypeTemp InteractionType { get; set; } = interaction;

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
