namespace WibboEmulator.Games.Groups;

public class GroupMember(int id, string username, string look)
{
    public int Id { get; set; } = id;
    public string Username { get; set; } = username;
    public string Look { get; set; } = look;
}
