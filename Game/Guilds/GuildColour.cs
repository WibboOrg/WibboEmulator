namespace Butterfly.Game.Guilds
{
    public class GuildColour
    {
        public int Id { get; private set; }
        public string Colour { get; private set; }

        public GuildColour(int id, string colour)
        {
            this.Id = id;
            this.Colour = colour;
        }
    }
}
