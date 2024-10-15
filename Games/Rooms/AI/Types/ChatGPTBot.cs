namespace WibboEmulator.Games.Rooms.AI.Types;

using System.Drawing;
using System.Text.RegularExpressions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Core;
using WibboEmulator.Core.OpenIA;
using WibboEmulator.Core.Settings;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Utilities;

public partial class ChatGPTBot : BotAI
{
    private const int MAX_HISTORY = 10;
    private readonly Dictionary<int, List<ChatCompletionMessage>> _userMessages;
    private RoomUser _lastMessageUser;
    private readonly List<string> _listUserName;
    private readonly Dictionary<int, string> _listActions;
    private int _resetBotTimer;
    private int _resetDanseTimer;
    private int _timeoutTimer = 10;

    public ChatGPTBot(int virtualId)
    {
        this.VirtualId = virtualId;
        this._userMessages = [];
        this._listUserName = [];

        this._listActions = new Dictionary<int, string>()
        {
            { 0, "Do nothing" },
            { 1, "Greet" },
            { 2, "Stand up" },
            { 3, "Give the user an ice cream" },
            { 4, "Sit down" },
            { 5, "Move to user" },
            { 6, "Love or like" },
            { 7, "Sad or bad or angry" },
            { 8, "Laugther" },
            { 9, "Kiss or embrace" },
            { 10, "Danse" },
        };
    }

    public override void OnSelfEnterRoom()
    {
    }

    private void StackMessages(int userId, params ChatCompletionMessage[] message)
    {
        if (this._userMessages.TryGetValue(userId, out var userMessages))
        {
            userMessages.AddRange(message);
            this._userMessages[userId] = userMessages.TakeLast(MAX_HISTORY).ToList();
        }
        else
        {
            this._userMessages.Add(userId, new List<ChatCompletionMessage>(message));
        }
    }

    public void RemoveUserMessages(int userId) => _ = this._userMessages.Remove(userId);

    public override void OnSelfLeaveRoom(bool kicked)
    {
    }

    public override void OnUserEnterRoom(RoomUser user)
    {
        if (!this._listUserName.Contains(user.Username))
        {
            this._listUserName.Add(user.Username);
        }
    }

    public override void OnUserLeaveRoom(GameClient client)
    {
        if (!this._listUserName.Contains(client.User.Username))
        {
            this._listUserName.Add(client.User.Username);
        }

        this.RemoveUserMessages(client.User.Id);
    }

    public override void OnUserSay(RoomUser user, string message)
    {
        if (this.BotData == null)
        {
            return;
        }

        var botName = this.BotData.Name;

        if (!message.Contains("@" + botName) && !message.StartsWith(": " + botName) && !message.StartsWith(botName))
        {
            return;
        }

        message = message.Replace(": " + botName, "");
        message = message.Trim();

        var chatMsg = new ChatCompletionMessage()
        {
            Content = message,
            Role = "user"
        };

        this.StackMessages(user.UserId, chatMsg);
        this._lastMessageUser = user;
    }

    public override void OnUserShout(RoomUser user, string message)
    {
    }

    private void ParseActionId(string messageText, int userId)
    {
        var regexMatch = MyRegex().Match(messageText);
        if (!regexMatch.Success || !regexMatch.Groups[1].Success ||
!int.TryParse(regexMatch.Groups[1].Value, out var actionId))
        {
            return;
        }

        var targetUser = this.Room.RoomUserManager.GetRoomUserByUserId(userId);
        if (targetUser == null || actionId == 0)
        {
            return;
        }

        switch (actionId)
        {
            case 1: //Greet
            {
                this.Room.SendPacket(new ActionComposer(this.RoomUser.VirtualId, 1));
                break;
            }
            case 2: //Sand up
            {
                if (this.RoomUser.ContainStatus("sit"))
                {
                    this.RoomUser.RemoveStatus("sit");
                    this.RoomUser.IsSit = false;
                    this.RoomUser.UpdateNeeded = true;
                }
                break;
            }
            case 3: //Give the user an ice cream
            {
                const int iceCreamId = 4;
                if (targetUser.CarryItemId != iceCreamId)
                {
                    targetUser.CarryItem(iceCreamId);
                }
                break;
            }
            case 4: //Sit down
            {
                if (this.RoomUser.ContainStatus("sit") || this.RoomUser.ContainStatus("lay"))
                {
                    break;
                }

                if (this.RoomUser.RotBody % 2 == 0)
                {
                    this.RoomUser.SetStatus("sit", "0.5");
                    this.RoomUser.IsSit = true;
                    this.RoomUser.UpdateNeeded = true;
                }
                break;
            }
            case 5: //Move to user
            {
                var numberCars = new List<int> { 22, 21, 17, 15, 6, 3, 2 };
                this.RoomUser.ApplyEffect(numberCars.GetRandomElement(), true);

                this.
                RoomUser.TimerResetEffect = 6;
                this.RoomUser.MoveTo(targetUser.X, targetUser.Y, true);

                this._resetBotTimer = 600;
                break;
            }
            case 6: //Love or like
            {
                this.RoomUser.ApplyEffect(168, true);
                this.RoomUser.TimerResetEffect = 6;
                break;
            }
            case 7: //Sad or bad or angry
            {
                this.RoomUser.ApplyEffect(113, true);
                this.RoomUser.TimerResetEffect = 6;
                break;
            }
            case 8: //Laugther
            {
                this.Room.SendPacket(new ActionComposer(this.RoomUser.VirtualId, 3));
                break;
            }
            case 9: //Kiss or embrace
            {
                this.Room.SendPacket(new ActionComposer(this.RoomUser.VirtualId, 2));
                break;
            }
            case 10: //Danse
            {
                var danceId = WibboEnvironment.GetRandomNumber(1, 4);
                if (danceId > 0 && this.RoomUser.CarryItemId > 0)
                {
                    this.RoomUser.CarryItem(0);
                }

                this.
                RoomUser.DanceId = danceId;
                this.Room.SendPacket(new DanceComposer(this.RoomUser.VirtualId, danceId));
                this._resetDanseTimer = 12;
                break;
            }
        }

        _ = this.Room.AllowsShous(this.RoomUser, this.BotData.Name + "_" + actionId);
    }

    public override void OnTimerTick()
    {
        if (this.BotData == null)
        {
            return;
        }

        if (this._resetDanseTimer > 0)
        {
            this._resetDanseTimer--;

            if (this._resetDanseTimer <= 0)
            {
                if (this.RoomUser.DanceId > 0)
                {
                    this.RoomUser.DanceId = 0;
                    this.Room.SendPacket(new DanceComposer(this.RoomUser.VirtualId, 0));
                }
            }
        }

        if (this._resetBotTimer > 0)
        {
            this._resetBotTimer--;

            if (this._resetBotTimer <= 0)
            {
                var bot = this.RoomUser;

                bot.RotHead = bot.BotData.Rot;
                bot.RotBody = bot.BotData.Rot;
                this.Room.SendPacket(RoomItemHandling.TeleportUser(bot, new Point(bot.BotData.X, bot.BotData.Y), 0, this.Room.GameMap.SqAbsoluteHeight(bot.BotData.X, bot.BotData.Y)));
            }
        }

        if (!OpenAIProxy.IsReadyToSendChat || this._lastMessageUser == null)
        {
            return;
        }

        if (this._timeoutTimer > 0)
        {
            this._timeoutTimer--;

            return;
        }

        var botName = this.BotData.Name;
        var userId = this._lastMessageUser.UserId;
        var userName = this._lastMessageUser.Username;
        var userGender = this._lastMessageUser.Client?.User?.Gender;

        this._lastMessageUser = null;
        this._timeoutTimer = 10;

        _ = this.Room.RunTask(async () =>
        {
            try
            {
                if (!OpenAIProxy.IsReadyToSendChat)
                {
                    return;
                }

                this.
                Room.SendPacket(new UserTypingComposer(this.VirtualId, true));

                var listActions = "";
                foreach (var kvp in this._listActions)
                {
                    listActions += $"{kvp.Key}: {kvp.Value}. ";
                }

                var firstPrompt = !string.IsNullOrWhiteSpace(this.BotData.ChatText) ? this.BotData.ChatText : SettingsManager.GetData<string>("openia.prompt");
                firstPrompt = firstPrompt.Replace("{{botname}}", botName);
                firstPrompt = firstPrompt.Replace("{{username}}", userName);
                firstPrompt = firstPrompt.Replace("{{usergender}}", userGender == "M" ? "man" : "women");
                firstPrompt = firstPrompt.Replace("{{listusername}}", string.Join(",", this._listUserName));
                firstPrompt = firstPrompt.Replace("{{currentdate}}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                firstPrompt = firstPrompt.Replace("{{listaction}}", listActions[..^1]);

                var prePrompt = new ChatCompletionMessage()
                {
                    Content = firstPrompt,
                    Role = "system"
                };
                var messagesSend = new List<ChatCompletionMessage>([prePrompt]);
                messagesSend.AddRange(this._userMessages.TryGetValue(userId, out var userMessages) ? userMessages : []);

                var messagesGtp = await OpenAIProxy.SendChatMessage(messagesSend);

                if (messagesGtp != null)
                {
                    var message = messagesGtp.Content;
                    if (message.Contains("(Action: "))
                    {
                        message = message.Split("(Action: ")[0];
                        this.ParseActionId(messagesGtp.Content, userId);
                    }

                    var chatTexts = SplitSentence(message, 20);

                    foreach (var chatText in chatTexts.Take(4))
                    {
                        if (!string.IsNullOrWhiteSpace(chatText))
                        {
                            this.RoomUser.OnChat(chatText.Length > 150 ? chatText[..150] + "..." : chatText);
                        }
                    }

                    this.StackMessages(userId, messagesGtp);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString());
            }
            finally
            {
                this.Room.SendPacket(new UserTypingComposer(this.VirtualId, false));
            }
        });
    }

    private static List<string> SplitSentence(string sentence, int chunkSize)
    {
        var words = sentence.Split(' ');
        var chunks = new List<string>();

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

    [GeneratedRegex("\\(Action: (\\d+)\\)", options: RegexOptions.IgnoreCase, "fr-BE")]
    private static partial Regex MyRegex();
}
