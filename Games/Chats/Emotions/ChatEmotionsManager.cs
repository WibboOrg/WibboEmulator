namespace WibboEmulator.Games.Chats.Emotions;

public static class ChatEmotionsManager
{
    private static readonly Dictionary<string, ChatEmotions> Emotions = new()
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
    /// <param name="text">The text to search through</param>
    /// <returns></returns>
    public static int GetEmotionsForText(string text)
    {
        foreach (var kvp in Emotions)
        {
            if (text.ToLower().Contains(kvp.Key.ToLower()))
            {
                return GetEmoticonPacketNum(kvp.Value);
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
