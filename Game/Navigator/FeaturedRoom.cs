using Butterfly.Core;

namespace Butterfly.Game.Navigator
{
    public class FeaturedRoom
    {
        public int RoomId { get; private set; }
        public string Image { get; private set; }
        public Language Langue { get; private set; }
        public bool Game { get; private set; }

        public FeaturedRoom(int RoomId, string Image, Language Langue, bool Game)
        {
            this.RoomId = RoomId;
            this.Image = Image;
            this.Langue = Langue;
            this.Game = Game;
        }
    }
}