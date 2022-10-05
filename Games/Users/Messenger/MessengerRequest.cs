namespace WibboEmulator.Games.Users.Messenger;

public class MessengerRequest
{
    public int To { get; }
    public int From { get; }
    public string Username { get; }

    public MessengerRequest(int toUser, int fromUser, string username)
    {
        this.To = toUser;
        this.From = fromUser;
        this.Username = username;
    }
}
