namespace WibboEmulator.Games.Groups;

public class GroupMember
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Look { get; set; }

    public GroupMember(int id, string username, string look)
    {
        this.Id = id;
        this.Username = username;
        this.Look = look;
    }
}
