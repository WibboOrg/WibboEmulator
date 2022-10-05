namespace WibboEmulator.Games.Rooms.AI.Speech;

public class RandomSpeech
{
    public int BotID { get; private set; }
    public string Message { get; private set; }

    public RandomSpeech(string message, int botID)
    {
        this.BotID = botID;
        this.Message = message;
    }
}
