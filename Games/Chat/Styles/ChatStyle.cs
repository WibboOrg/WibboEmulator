namespace WibboEmulator.Games.Chat.Styles;

public sealed class ChatStyle
{
    public ChatStyle(int Id, string Name, string RequiredRight)
    {
        this.Id = Id;
        this.Name = Name;
        this.RequiredRight = RequiredRight;
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public string RequiredRight { get; set; }
}