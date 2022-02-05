namespace Butterfly.Game.Users.Messenger
{
    public struct SearchResult
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Look { get; set; }

        public SearchResult(int userID, string username, string look)
        {
            this.UserId = userID;
            this.Username = username;
            this.Look = look;
        }
    }
}
