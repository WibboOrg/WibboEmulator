namespace Wibbo.Game.Users.Wardrobes
{
    public class Wardrobe
    {
        public int SlotId { get; private set; }
        public string Look { get; private set; }
        public string Gender { get; private set; }

        public Wardrobe(int slotId, string look, string gender)
        {
            this.SlotId = slotId;
            this.Look = look;
            this.Gender = gender;
        }
    }
}
