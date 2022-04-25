namespace Butterfly.Game.Users.Messenger
{
    public struct SearchResult
    {
        public int UserId;
        public string Username;
        public string Motto;
        public string LastOnline;
        public string Look;

        public SearchResult(int UserId, string Username, string Motto, string LastOnline, string Look)
        {
            this.UserId = UserId;
            this.Username = Username;
            this.Motto = Motto;
            this.LastOnline = LastOnline;
            this.Look = Look;
        }
    }
}
