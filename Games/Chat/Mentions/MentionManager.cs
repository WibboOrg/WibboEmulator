namespace WibboEmulator.Games.Chat.Mentions;
using System.Text.RegularExpressions;
using WibboEmulator.Communication.Packets.Outgoing.Notifications.NotifCustom;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Games.GameClients;

public class MentionManager
{
    private readonly string _mentionPattern = @"@(.*?)(?=\s|$)";

    private readonly string _stylePrefix = "[tag]";
    private readonly string _styleSuffix = "[/tag]";

    public string Parse(GameClient session, string message)
    {
        var styledMessage = message;

        var changeLength = this._stylePrefix.Length + this._styleSuffix.Length;
        var changeCount = 0;
        var usersTarget = new List<string>();

        foreach (var m in Regex.Matches(message, this._mentionPattern).Cast<Match>())
        {
            var targetUsername = m.Groups[1].Value.ToLower();

            if (string.IsNullOrWhiteSpace(targetUsername))
            {
                continue;
            }

            if (usersTarget.Contains(targetUsername))
            {
                continue;
            }

            usersTarget.Add(targetUsername);

            if (targetUsername == "everyone")
            {
                if (!EveryoneFriend(session, message))
                {
                    break;
                }
            }

            else if (!SendNotif(session, targetUsername, message))
            {
                continue;
            }

            styledMessage = styledMessage.Insert((changeCount * changeLength) + m.Index, this._stylePrefix);
            styledMessage = styledMessage.Insert((changeCount * changeLength) + m.Index + this._stylePrefix.Length + m.Length, this._styleSuffix);

            changeCount++;
        }

        return styledMessage;
    }

    public static bool SendNotif(GameClient session, string targetUsername, string message)
    {
        var targetClient = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(targetUsername);

        if (targetClient == null)
        {
            return false;
        }

        if (targetClient == session)
        {
            return false;
        }

        if (targetClient.User == null || targetClient.User.Messenger == null)
        {
            return false;
        }

        if (!targetClient.User.Messenger.FriendshipExists(session.User.Id) && !session.User.HasPermission("perm_mention"))
        {
            session.SendPacket(RoomNotificationComposer.SendBubble("error", $"Tu as besoin d'être ami avec {targetUsername} pour pouvoir le taguer"));
            return false;
        }

        targetClient.SendPacket(new MentionComposer(session.User.Id, session.User.Username, session.User.Look, message));

        return true;
    }

    public static bool EveryoneFriend(GameClient session, string message)
    {
        if (session.User.Rank < 2)
        {
            session.SendPacket(RoomNotificationComposer.SendBubble("error", $"Vous devez être Premium pour utiliser @everyone"));
            return false;
        }

        var timeSpan = DateTime.Now - session.User.EveryoneTimer;
        if (timeSpan.TotalSeconds < 120)
        {
            session.SendPacket(RoomNotificationComposer.SendBubble("error", $"Veuille attendre 2 minute avant de pouvoir réutiliser @everyone"));
            return false;
        }

        session.
        User.EveryoneTimer = DateTime.Now;

        var onlineUsers = WibboEnvironment.GetGame().GetGameClientManager().GetClientsById(session.User.Messenger.Friends.Keys);

        if (onlineUsers == null)
        {
            return false;
        }

        foreach (var targetClient in onlineUsers)
        {
            if (targetClient != null && targetClient.User != null && targetClient.User.Messenger != null)
            {
                if (targetClient.User.Messenger.FriendshipExists(session.User.Id))
                {
                    targetClient.SendPacket(new MentionComposer(session.User.Id, session.User.Username, session.User.Look, message));
                }
            }
        }

        return true;
    }
}
