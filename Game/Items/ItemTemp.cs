using Butterfly.Game.Rooms.Map.Movement;

namespace Butterfly.Game.Items
{
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

        public ItemTemp(int id, int userId, int spriteId, int x, int y, double z, string extraData, MovementDirection movement, int value, InteractionTypeTemp pInteraction, int pDistance = 0, int pTeamId = 0)
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
            this.TeamId = pTeamId;
            this.Distance = pDistance;
            this.InteractionType = pInteraction;
        }

        public bool Equals(ItemTemp comparedItem)
        {
            return comparedItem.Id == this.Id;
        }
    }

    public enum InteractionTypeTemp
    {
        NONE,
        PROJECTILE,
        PROJECTILE_BOT,
        GRENADE,
        MONEY,
        RPITEM,
    }
}
