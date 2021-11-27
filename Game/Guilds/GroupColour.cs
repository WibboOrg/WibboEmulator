namespace Butterfly.Game.Guilds
{
    public class GroupColour
    {
        public int Id { get; private set; }
        public string Colour { get; private set; }

        public GroupColour(int id, string colour)
        {
            this.Id = id;
            this.Colour = colour;
        }
    }
}
