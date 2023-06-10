namespace WibboEmulator.Games.Rooms.AI.Types;

using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Games.GameClients;

public class OpenAIBot : BotAI
{
    private int _tickCount;
    private bool _waitedAPI;
    private int _lastMessageUserId;
    private string _lastMessageUserName;
    private string _lastMessageText;

    public OpenAIBot(int virtualId) => this.VirtualId = virtualId;

    public override void OnSelfEnterRoom()
    {
    }

    public override void OnSelfLeaveRoom(bool kicked)
    {
    }

    public override void OnUserEnterRoom(RoomUser user)
    {
    }

    public override void OnUserLeaveRoom(GameClient client) => WibboEnvironment.GetChatOpenAI().RemoveUserMessages(client.User.Id);

    public override void OnUserSay(RoomUser user, string message)
    {
        if (this.GetBotData() == null || message.Length < 10)
        {
            return;
        }

        var botName = this.GetBotData().Name;

        if (!message.Contains("@" + botName) && !message.StartsWith(": " + botName) && !message.StartsWith(botName))
        {
            return;
        }

        message = message.Replace(": " + botName, "");
        message = message.Trim();

        this._lastMessageUserId = user.UserId;
        this._lastMessageUserName = user.GetUsername();
        this._lastMessageText = message;
    }

    public override void OnUserShout(RoomUser user, string message)
    {
    }

    public override void OnTimerTick()
    {
        this._tickCount++;

        if (this.GetBotData() == null || this._tickCount < 40 || this._waitedAPI || this._lastMessageUserId == 0 || this._lastMessageUserId == 0)
        {
            return;
        }

        var userId = this._lastMessageUserId;
        var userName = this._lastMessageUserName;
        var message = this._lastMessageText;

        this._tickCount = 0;
        this._waitedAPI = true;
        this._lastMessageText = "";
        this._lastMessageUserName = "";
        this._lastMessageUserId = 0;

        this.GetRoom().SendPacket(new UserTypingComposer(this.VirtualId, true));

        _ = this.GetRoom().RunTask(async () =>
        {
            try
            {
                var results = await WibboEnvironment.GetChatOpenAI().SendChatMessage(userId, userName, message);

                if (results != null)
                {
                    foreach (var item in results)
                    {
                        var chatText = item.Content.StartsWith("ChatGPT:") ? item.Content.Split("ChatGPT:")[1] : item.Content;

                        var chunks = SplitSentence(chatText, 20);

                        foreach (var chunk in chunks)
                        {
                            this.GetRoomUser().OnChat(chunk);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                this.GetRoom().SendPacket(new UserTypingComposer(this.VirtualId, false));
                this._waitedAPI = false;
            }
        });
    }

    public static List<string> SplitSentence(string sentence, int chunkSize)
    {
        var words = sentence.Split(' '); // Divise la phrase en mots individuels
        var chunks = new List<string>(); // Liste pour stocker les morceaux de la phrase

        var startIndex = 0;
        while (startIndex < words.Length)
        {
            var endIndex = Math.Min(startIndex + chunkSize, words.Length);
            var chunk = string.Join(" ", words, startIndex, endIndex - startIndex);
            chunks.Add(chunk);
            startIndex = endIndex;
        }

        return chunks;
    }
}
