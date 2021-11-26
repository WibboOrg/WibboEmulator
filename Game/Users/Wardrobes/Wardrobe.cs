namespace Butterfly.Game.Users.Wardrobes
{
    public class Wardrobe
    {
        public int SlotId { get; set; }
        public string Look { get; set; }
        public string Gender { get; set; }

        public Wardrobe(int slotId, string look, string gender)
        {
            this.SlotId = slotId;
            this.Look = look;
            this.Gender = gender;
        }
    }
}
