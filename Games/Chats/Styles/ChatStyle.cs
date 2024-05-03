namespace WibboEmulator.Games.Chats.Styles;

public sealed class ChatStyle(int id, string name, string requiredRight)
{
    public int Id { get; set; } = id;

    public string Name { get; set; } = name;

    public string RequiredRight { get; set; } = requiredRight;
}
