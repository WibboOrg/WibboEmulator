namespace WibboEmulator.Games.Moderations;

public class HelpCategory(int id, string caption)
{
    public int CategoryId { get; private set; } = id;
    public string Caption { get; private set; } = caption;
}
