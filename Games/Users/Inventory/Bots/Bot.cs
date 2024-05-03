namespace WibboEmulator.Games.Users.Inventory.Bots;

using WibboEmulator.Games.Rooms.AI;

public class Bot(int id, int ownerId, string name, string motto, string figure, string gender, bool walkingEnabled, bool chatEnabled, string chatText, int chatSeconds, bool isDancing, int enable, int handitem, int status, BotAIType aiType)
{
    public int Id { get; set; } = id;
    public int OwnerId { get; set; } = ownerId;
    public string Name { get; set; } = name;
    public string Motto { get; set; } = motto;
    public string Figure { get; set; } = figure;
    public string Gender { get; set; } = gender;
    public bool WalkingEnabled { get; set; } = walkingEnabled;
    public bool ChatEnabled { get; set; } = chatEnabled;
    public string ChatText { get; set; } = chatText;
    public int ChatSeconds { get; set; } = chatSeconds;
    public bool IsDancing { get; set; } = isDancing;
    public int Enable { get; set; } = enable;
    public int Handitem { get; set; } = handitem;
    public int Status { get; set; } = status;
    public BotAIType AIType { get; set; } = aiType;
}
