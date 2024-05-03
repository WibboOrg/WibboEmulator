namespace WibboEmulator.Games.Users.Messenger;

public class MessengerRequest(int toUser, int fromUser, string username)
{
    public int To { get; } = toUser;
    public int From { get; } = fromUser;
    public string Username { get; } = username;
}
