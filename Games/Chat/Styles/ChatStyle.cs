namespace WibboEmulator.Games.Chat.Styles;

public sealed class ChatStyle
{
    public ChatStyle(int id, string name, string requiredRight)
    {
        this.Id = id;
        this.Name = name;
        this.RequiredRight = requiredRight;
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public string RequiredRight { get; set; }
}
