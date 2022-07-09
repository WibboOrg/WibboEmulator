namespace Wibbo.Game.Rooms.AI.Speech
{
    public class RandomSpeech
    {
        public int BotID;
        public string Message;

        public RandomSpeech(string Message, int BotID)
        {
            this.BotID = BotID;
            this.Message = Message;
        }
    }
}