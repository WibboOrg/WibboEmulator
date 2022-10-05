namespace WibboEmulator.Games.Chat.Emotions;

public sealed class ChatEmotionsManager
{
    private readonly Dictionary<string, ChatEmotions> Emotions = new()
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
        foreach (var Kvp in this.Emotions)
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
    private static int GetEmoticonPacketNum(ChatEmotions e)
    {
        switch (e)
        {
            case ChatEmotions.SMILE:
                return 1;

            case ChatEmotions.ANGRY:
                return 2;

            case ChatEmotions.SHOCKED:
                return 3;

            case ChatEmotions.SAD:
                return 4;

            case ChatEmotions.NONE:
            default:
                return 0;
        }
    }
}
