namespace Butterfly.Game.Guilds
{
    public class GuildBadgePart
    {
        public int Id { get; private set; }
        public string AssetOne { get; private set; }
        public string AssetTwo { get; private set; }

        public GuildBadgePart(int id, string assetOne, string assetTwo)
        {
            this.Id = id;
            this.AssetOne = assetOne;
            this.AssetTwo = assetTwo;
        }
    }
}
