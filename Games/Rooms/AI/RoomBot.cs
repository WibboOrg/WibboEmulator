namespace WibboEmulator.Games.Rooms.AI;
using WibboEmulator.Games.Roleplays.Enemy;
using WibboEmulator.Games.Rooms.AI.Types;

public class RoomBot
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string Name { get; set; }
    public string Motto { get; set; }
    public string Gender { get; set; }
    public string Look { get; set; }
    public int RoomId { get; set; }
    public bool WalkingEnabled { get; set; }
    public int FollowUser { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public double Z { get; set; }
    public int Rot { get; set; }
    public bool AutomaticChat { get; set; }
    public string ChatText { get; set; }
    public List<string> RandomSpeech { get; set; }
    public int SpeakingInterval { get; set; }
    public bool MixSentences { get; set; }

    public int Enable { get; set; }
    public int Handitem { get; set; }
    public int Status { get; set; }

    public bool IsDancing { get; set; }
    public BotAIType AiType { get; set; }
    public RoleBot RoleBot { get; set; }

    public bool IsPet => this.AiType is BotAIType.Pet or BotAIType.RoleplayPet;

    public string OwnerName => WibboEnvironment.GetGame().GetGameClientManager().GetNameById(this.OwnerId);

    public RoomBot(int botId, int ownerId, int roomId, BotAIType aiType, bool walkingEnabled, string name, string motto, string gender, string look, int x, int y, double z, int rot, bool chatEnabled, string chatText, int chatSeconds, bool isDancing, int effectEnable, int handitemId, int status)
    {
        this.Id = botId;
        this.OwnerId = ownerId;
        this.RoomId = roomId;

        this.AiType = aiType;
        this.RoleBot = null;

        this.Name = name;
        this.Motto = motto;
        this.Gender = gender;
        this.Look = look;

        this.X = x;
        this.Y = y;
        this.Z = z;
        this.Rot = rot;

        this.WalkingEnabled = walkingEnabled;
        this.AutomaticChat = chatEnabled;
        this.ChatText = chatText;
        this.SpeakingInterval = chatSeconds;
        this.IsDancing = isDancing;
        this.MixSentences = false;
        this.Enable = effectEnable;
        this.Handitem = handitemId;
        this.Status = status;

        this.RandomSpeech = new List<string>();

        this.LoadRandomSpeech(this.ChatText);
    }

    public void LoadRandomSpeech(string text)
    {
        if (!text.Contains('\r'))
        {
            return;
        }

        if (this.RandomSpeech.Count > 0)
        {
            this.RandomSpeech.Clear();
        }

        foreach (var message in text.Split('\r'))
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                this.RandomSpeech.Add((message.Length > 150) ? message[..150] : message);
            }
        }
    }

    public string GetRandomSpeech() => this.RandomSpeech[WibboEnvironment.GetRandomNumber(0, this.RandomSpeech.Count - 1)];

    public BotAI GenerateBotAI(int virtualId)
    {
        switch (this.AiType)
        {
            case BotAIType.RoleplayBot:
            case BotAIType.RoleplayPet:
                return new RoleplayBot(virtualId);
            case BotAIType.SuperBot:
                return new SuperBot(virtualId);
            case BotAIType.Pet:
                return new PetBot(virtualId);
            case BotAIType.ChatGPT:
                return new ChatGPTBot(virtualId);
            case BotAIType.Generic:
                break;
        }

        return new GenericBot(virtualId);
    }
}
