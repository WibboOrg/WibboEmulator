using Butterfly.Core;

namespace Butterfly.Game.Navigators
{
    public class FeaturedRoom
    {
        public int RoomId;
        public string Image;
        public Language Langue;
        public bool Game;

        public FeaturedRoom(int RoomId, string Image, Language Langue, bool Game)
        {
            this.RoomId = RoomId;
            this.Image = Image;
            this.Langue = Langue;
            this.Game = Game;
        }
    }
}