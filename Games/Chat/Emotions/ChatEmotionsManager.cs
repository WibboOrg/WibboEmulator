namespace WibboEmulator.Games.Chat.Emotions;

public sealed class ChatEmotionsManager
{
    private readonly Dictionary<string, ChatEmotions> _emotions = new()
        {
            // Smile
            { ":)", ChatEmotions.SMILE },
            { ";)", ChatEmotions.SMILE },
            { ":d", ChatEmotions.SMILE },
            { ";d", ChatEmotions.SMILE },
            { ":]", ChatEmotions.SMILE },
            { ";]", ChatEmotions.SMILE },
            { "=)", ChatEmotions.SMILE },
            { "=]", ChatEmotions.SMILE },
            { ":-)", ChatEmotions.SMILE },
 
            // Angry
            { ">:(", ChatEmotions.ANGRY },
            { ">:[", ChatEmotions.ANGRY },
            { ">;[", ChatEmotions.ANGRY },
            { ">;(", ChatEmotions.ANGRY },
            { ">=(", ChatEmotions.ANGRY },
            { ":@", ChatEmotions.ANGRY },
 
            // Shocked
            { ":o", ChatEmotions.SHOCKED },
            { ";o", ChatEmotions.SHOCKED },
            { ">;o", ChatEmotions.SHOCKED },
            { ">:o", ChatEmotions.SHOCKED },
            { "=o", ChatEmotions.SHOCKED },
            { ">=o", ChatEmotions.SHOCKED },
 
            // Sad
            { ";'(", ChatEmotions.SAD },
            { ";[", ChatEmotions.SAD },
            { ":[", ChatEmotions.SAD },
            { ";(", ChatEmotions.SAD },
            { "=(", ChatEmotions.SAD },
            { "='(", ChatEmotions.SAD },
            { "=[", ChatEmotions.SAD },
            { "='[", ChatEmotions.SAD },
            { ":(", ChatEmotions.SAD },
            { ":-(", ChatEmotions.SAD }
        };

    /// <summary>
    /// Searches the provided text for any emotions that need to be applied and returns the packet number.
    /// </summary>
    /// <param name="Text">The text to search through</param>
    /// <returns></returns>
    public int GetEmotionsForText(string Text)
    {
        foreach (var Kvp in this._emotions)
        {
            if (Text.ToLower().Contains(Kvp.Key.ToLower()))
            {
                return GetEmoticonPacketNum(Kvp.Value);
            }
        }

        return 0;
    }

    /// <summary>
    /// Trys to get the packet number for the provided chat emotion.
    /// </summary>
    /// <param name="e">Chat Emotion</param>
    /// <returns></returns>
    private static int GetEmoticonPacketNum(ChatEmotions e) => e switch
    {
        ChatEmotions.SMILE => 1,
        ChatEmotions.ANGRY => 2,
        ChatEmotions.SHOCKED => 3,
        ChatEmotions.SAD => 4,
        _ => 0,
    };
}
