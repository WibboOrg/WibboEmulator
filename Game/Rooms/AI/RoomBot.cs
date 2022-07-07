using WibboEmulator.Game.Roleplay.Enemy;
using WibboEmulator.Game.Rooms.AI.Types;

namespace WibboEmulator.Game.Rooms.AI
{
    public class RoomBot
    {
        public int Id;
        public int OwnerId;
        public string Name;
        public string Motto;
        public string Gender;
        public string Look;
        public int RoomId;
        public bool WalkingEnabled;
        public int FollowUser;
        public int X;
        public int Y;
        public double Z;
        public int Rot;
        public bool AutomaticChat;
        public string ChatText;
        public List<string> RandomSpeech;
        public int SpeakingInterval;
        public bool MixSentences;

        public int Enable;
        public int Handitem;
        public int Status;

        public bool IsDancing;
        public BotAIType AiType;
        public RoleBot RoleBot;

        public bool IsPet => this.AiType == BotAIType.Pet || this.AiType == BotAIType.RoleplayPet;

        public string OwnerName => WibboEnvironment.GetGame().GetClientManager().GetNameById(this.OwnerId);

        public RoomBot(int BotId, int OwnerId, int RoomId, BotAIType AiType, bool WalkingEnabled, string Name, string Motto, string Gender, string Look, int X, int Y, double Z, int Rot, bool ChatEnabled, string ChatText, int ChatSeconds, bool IsDancing, int pEffectEnable, int pHanditemId, int pStatus)
        {
            this.Id = BotId;
            this.OwnerId = OwnerId;
            this.RoomId = RoomId;

            this.AiType = AiType;
            this.RoleBot = null;

            this.Name = Name;
            this.Motto = Motto;
            this.Gender = Gender;
            this.Look = Look;

            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.Rot = Rot;

            this.WalkingEnabled = WalkingEnabled;
            this.AutomaticChat = ChatEnabled;
            this.ChatText = ChatText;
            this.SpeakingInterval = ChatSeconds;
            this.IsDancing = IsDancing;
            this.MixSentences = false;
            this.Enable = pEffectEnable;
            this.Handitem = pHanditemId;
            this.Status = pStatus;

            this.RandomSpeech = new List<string>();

            this.LoadRandomSpeech(this.ChatText);
        }

        public void LoadRandomSpeech(string Text)
        {
            if (!Text.Contains('\r'))
            {
                return;
            }

            if (this.RandomSpeech.Count > 0)
            {
                this.RandomSpeech.Clear();
            }

            foreach (string Message in Text.Split(new char[] { '\r' }))
            {
                if (!string.IsNullOrWhiteSpace(Message))
                {
                    this.RandomSpeech.Add((Message.Length > 150) ? Message.Substring(0, 150) : Message);
                }
            }
        }

        public string GetRandomSpeech()
        {
            return this.RandomSpeech[WibboEnvironment.GetRandomNumber(0, this.RandomSpeech.Count - 1)];
        }

        public BotAI GenerateBotAI(int VirtualId)
        {
            switch (this.AiType)
            {
                case BotAIType.RoleplayBot:
                case BotAIType.RoleplayPet:
                    return new RoleplayBot(VirtualId);
                case BotAIType.SuperBot:
                    return new SuperBot(VirtualId);
                case BotAIType.Pet:
                    return new PetBot(VirtualId);
                case BotAIType.Generic:
                    break;
                case BotAIType.CopyBot:
                    break;
            }

            return new GenericBot(VirtualId);
        }
    }
}
