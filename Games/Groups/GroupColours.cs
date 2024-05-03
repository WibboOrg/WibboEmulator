namespace WibboEmulator.Games.Groups;

public class GroupColours(int id, string colour)
{
    public int Id { get; private set; } = id;
    public string Colour { get; private set; } = colour;
}
