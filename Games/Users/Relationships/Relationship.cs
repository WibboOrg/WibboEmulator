namespace WibboEmulator.Games.Users.Relationships;

public class Relationship
{
    public int UserId { get; private set; }
    public int Type { get; set; }

    public Relationship(int user, int type)
    {
        this.UserId = user;
        this.Type = type;
    }
}
