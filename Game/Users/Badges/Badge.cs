namespace Wibbo.Game.Users.Badges
{
    public class Badge
    {
        public string Code { get; private set; }
        public int Slot { get; set; }

        public Badge(string Code, int Slot)
        {
            this.Code = Code;
            this.Slot = Slot;
        }
    }
}