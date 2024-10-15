namespace WibboEmulator.Games.Chats.Mentions;
using System.Text.RegularExpressions;
using WibboEmulator.Communication.Packets.Outgoing.Notifications.NotifCustom;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Games.GameClients;

public static class MentionManager
{
    private static readonly string MentionPattern = @"@(.*?)(?=\s|$)";

    private static readonly string StylePrefix = "[tag]";
    private static readonly string StyleSuffix = "[/tag]";

    public static string Parse(GameClient Session, string message)
    {
        var styledMessage = message;

        var changeLength = StylePrefix.Length + StyleSuffix.Length;
        var changeCount = 0;
        var usersTarget = new List<string>();

        foreach (var m in Regex.Matches(message, MentionPattern).Cast<Match>())
        {
            var TargetUsername = m.Groups[1].Value.ToLower();

            if (string.IsNullOrWhiteSpace(TargetUsername))
            {
                continue;
            }

            if (usersTarget.Contains(TargetUsername))
            {
                continue;
            }

            usersTarget.Add(TargetUsername);

            if (TargetUsername == "everyone")
            {
                if (!EveryoneFriend(Session, message))
                {
                    break;
                }
            }
            else if (TargetUsername == "here")
            {
                if (!HereRoom(Session, message))
                {
                    break;
                }
            }
            else if (!SendNotification(Session, TargetUsername, message))
            {
                continue;
            }

            styledMessage = styledMessage.Insert((changeCount * changeLength) + m.Index, StylePrefix);
            styledMessage = styledMessage.Insert((changeCount * changeLength) + m.Index + StylePrefix.Length + m.Length, StyleSuffix);

            changeCount++;
        }

        return styledMessage;
    }

    public static bool SendNotification(GameClient Session, string TargetUsername, string message)
    {
        var targetClient = GameClientManager.GetClientByUsername(TargetUsername);

        if (targetClient == null)
        {
            return false;
        }

        if (targetClient == Session)
        {
            return false;
        }

        if (targetClient.User == null || targetClient.User.Messenger == null)
        {
            return false;
        }

        if (!targetClient.User.Messenger.FriendshipExists(Session.User.Id) && !Session.User.HasPermission("mention"))
        {
            Session.SendPacket(RoomNotificationComposer.SendBubble("error", $"Tu as besoin d'être ami avec {TargetUsername} pour pouvoir le taguer"));
            return false;
        }

        targetClient.SendPacket(new MentionComposer(Session.User.Id, Session.User.Username, Session.User.Look, message));

        return true;
    }

    public static bool EveryoneFriend(GameClient Session, string message)
    {
        if (Session.User.Rank < 2)
        {
            Session.SendPacket(RoomNotificationComposer.SendBubble("error", $"Vous devez être Premium pour utiliser @everyone"));
            return false;
        }

        var timeSpan = DateTime.Now - Session.User.EveryoneTimer;
        if (timeSpan.TotalSeconds < 120)
        {
            Session.SendPacket(RoomNotificationComposer.SendBubble("error", $"Veuillez patienter pendant 2 minutes avant de pouvoir réutiliser @everyone."));
            return false;
        }

        Session.User.EveryoneTimer = DateTime.Now;

        var onlineUsers = GameClientManager.GetClientsById(Session.User.Messenger.Friends.Keys);

        if (onlineUsers == null)
        {
            return false;
        }

        foreach (var targetClient in onlineUsers)
        {
            if (targetClient != null && targetClient.User != null && targetClient.User.Messenger != null)
            {
                if (targetClient.User.Messenger.FriendshipExists(Session.User.Id))
                {
                    targetClient.SendPacket(new MentionComposer(Session.User.Id, Session.User.Username, Session.User.Look, message));
                }
            }
        }

        return true;
    }

    public static bool HereRoom(GameClient Session, string message)
    {
        if (Session == null || Session.User == null || !Session.User.InRoom)
        {
            return false;
        }

        var room = Session.User.Room;
        if (room == null)
        {
            return false;
        }

        if (!room.CheckRights(Session) && !Session.User.HasPermission("mention_here"))
        {
            Session.SendPacket(RoomNotificationComposer.SendBubble("error", $"Vous devez être le propriétaire de l'appart pour @here"));
            return false;
        }

        var timeSpan = DateTime.Now - Session.User.HereTimer;
        if (timeSpan.TotalSeconds < 120)
        {
            Session.SendPacket(RoomNotificationComposer.SendBubble("error", $"Veuillez patienter pendant 2 minutes avant de pouvoir réutiliser @here."));
            return false;
        }

        Session.User.HereTimer = DateTime.Now;

        var onlineUsers = room.RoomUserManager.UserList;

        if (onlineUsers == null)
        {
            return false;
        }

        foreach (var targetClient in onlineUsers)
        {
            if (targetClient != null && targetClient.Client != null && targetClient.UserId != Session.User.Id)
            {
                targetClient.Client.SendPacket(new MentionComposer(Session.User.Id, Session.User.Username, Session.User.Look, message));
            }
        }

        return true;
    }
}
