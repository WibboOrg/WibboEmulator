namespace WibboEmulator.Games.Users.Inventory.Bots;

using WibboEmulator.Games.Rooms.AI;

public class Bot
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string Name { get; set; }
    public string Motto { get; set; }
    public string Figure { get; set; }
    public string Gender { get; set; }
    public bool WalkingEnabled { get; set; }
    public bool ChatEnabled { get; set; }
    public string ChatText { get; set; }
    public int ChatSeconds { get; set; }
    public bool IsDancing { get; set; }
    public int Enable { get; set; }
    public int Handitem { get; set; }
    public int Status { get; set; }
    public BotAIType AIType { get; set; }

    public Bot(int id, int ownerId, string name, string motto, string figure, string gender, bool walkingEnabled, bool chatEnabled, string chatText, int chatSeconds, bool isDancing, int enable, int handitem, int status, BotAIType aiType)
    {
        this.Id = id;
        this.OwnerId = ownerId;
        this.Name = name;
        this.Motto = motto;
        this.Figure = figure;
        this.Gender = gender;
        this.WalkingEnabled = walkingEnabled;
        this.ChatEnabled = chatEnabled;
        this.ChatText = chatText;
        this.ChatSeconds = chatSeconds;
        this.IsDancing = isDancing;
        this.Enable = enable;
        this.Handitem = handitem;
        this.Status = status;
        this.AIType = aiType;
    }
}
