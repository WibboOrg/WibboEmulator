namespace WibboEmulator.Games.Users.Messenger;

public struct SearchResult(int userId, string username, string motto, int lastOnline, string look)
{
    public int UserId { get; private set; } = userId;
    public string Username { get; private set; } = username;
    public string Motto { get; private set; } = motto;
    public int LastOnline { get; private set; } = lastOnline;
    public string Look { get; private set; } = look;
}
