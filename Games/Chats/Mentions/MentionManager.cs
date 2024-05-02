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

    public static string Parse(GameClient session, string message)
    {
        var styledMessage = message;

        var changeLength = StylePrefix.Length + StyleSuffix.Length;
        var changeCount = 0;
        var usersTarget = new List<string>();

        foreach (var m in Regex.Matches(message, MentionPattern).Cast<Match>())
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
            else if (targetUsername == "here")
            {
                if (!HereRoom(session, message))
                {
                    break;
                }
            }
            else if (!SendNotification(session, targetUsername, message))
            {
                continue;
            }

            styledMessage = styledMessage.Insert((changeCount * changeLength) + m.Index, StylePrefix);
            styledMessage = styledMessage.Insert((changeCount * changeLength) + m.Index + StylePrefix.Length + m.Length, StyleSuffix);

            changeCount++;
        }

        return styledMessage;
    }

    public static bool SendNotification(GameClient session, string targetUsername, string message)
    {
        var targetClient = GameClientManager.GetClientByUsername(targetUsername);

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

        if (!targetClient.User.Messenger.FriendshipExists(session.User.Id) && !session.User.HasPermission("mention"))
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
            session.SendPacket(RoomNotificationComposer.SendBubble("error", $"Veuillez patienter pendant 2 minutes avant de pouvoir réutiliser @everyone."));
            return false;
        }

        session.User.EveryoneTimer = DateTime.Now;

        var onlineUsers = GameClientManager.GetClientsById(session.User.Messenger.Friends.Keys);

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

    public static bool HereRoom(GameClient session, string message)
    {
        if (session == null || session.User == null || !session.User.InRoom)
        {
            return false;
        }

        var room = session.User.Room;
        if (room == null)
        {
            return false;
        }

        if (!room.CheckRights(session) && !session.User.HasPermission("mention_here"))
        {
            session.SendPacket(RoomNotificationComposer.SendBubble("error", $"Vous devez être le propriétaire de l'appart pour @here"));
            return false;
        }

        var timeSpan = DateTime.Now - session.User.HereTimer;
        if (timeSpan.TotalSeconds < 120)
        {
            session.SendPacket(RoomNotificationComposer.SendBubble("error", $"Veuillez patienter pendant 2 minutes avant de pouvoir réutiliser @here."));
            return false;
        }

        session.User.HereTimer = DateTime.Now;

        var onlineUsers = room.RoomUserManager.UserList;

        if (onlineUsers == null)
        {
            return false;
        }

        foreach (var targetClient in onlineUsers)
        {
            if (targetClient != null && targetClient.Client != null && targetClient.UserId != session.User.Id)
            {
                targetClient.Client.SendPacket(new MentionComposer(session.User.Id, session.User.Username, session.User.Look, message));
            }
        }

        return true;
    }
}
