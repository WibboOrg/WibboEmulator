namespace Butterfly.Game.User.Messenger
{
    public class Relationship
    {
        public int UserId;
        public int Type;

        public Relationship(int User, int Type)
        {
            this.UserId = User;
            this.Type = Type;
        }
    }
}
