namespace Butterfly.Game.Users.Badges
{
    public class Badge
    {
        public string Code { get; set; }
        public int Slot { get; set; }

        public Badge(string Code, int Slot)
        {
            this.Code = Code;
            this.Slot = Slot;
        }
    }
}