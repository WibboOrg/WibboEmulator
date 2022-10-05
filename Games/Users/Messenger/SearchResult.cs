namespace WibboEmulator.Games.Users.Messenger;

public struct SearchResult
{
    public int UserId { get; private set; }
    public string Username { get; private set; }
    public string Motto { get; private set; }
    public int LastOnline { get; private set; }
    public string Look { get; private set; }

    public SearchResult(int userId, string username, string motto, int lastOnline, string look)
    {
        this.UserId = userId;
        this.Username = username;
        this.Motto = motto;
        this.LastOnline = lastOnline;
        this.Look = look;
    }
}
