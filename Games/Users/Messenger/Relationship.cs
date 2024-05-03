namespace WibboEmulator.Games.Users.Messenger;

public class Relationship(int user, int type)
{
    public int UserId { get; private set; } = user;
    public int Type { get; set; } = type;
}
