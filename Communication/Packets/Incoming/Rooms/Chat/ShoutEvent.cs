namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Chat;

using System.Text.RegularExpressions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;
using WibboEmulator.Utilities;

internal sealed partial class ShoutEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null || !session.User.InRoom)
        {
            return;
        }

        var room = session.User.CurrentRoom;
        if (room == null)
        {
            return;
        }

        var user = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        if (user == null)
        {
            return;
        }

        if (room.IsRoleplay)
        {
            var rp = user.Roleplayer;
            if (rp != null && rp.Dead)
            {
                return;
            }
        }

        var message = StringCharFilter.Escape(packet.PopString());
        if (message.Length > 100)
        {
            message = message[..100];
        }

        var color = packet.PopInt();

        if (!WibboEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(color, out var style) || (style.RequiredRight.Length > 0 && !session.User.HasPermission(style.RequiredRight)))
        {
            color = 0;
        }

        if (color != 23)
        {
            message = StringCharFilter.Escape(message);
        }

        if (color == 23)
        {
            color = session.User.BadgeComponent.GetStaffBulleId();
        }

        user.Unidle();

        if (!session.User.HasPermission("mod") && !user.IsOwner() && !room.CheckRights(session) && room.RoomMuted)
        {
            user.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("room.muted", session.Langue));
            return;
        }

        if (room.JankenManager.PlayerStarted(user))
        {
            if (!room.JankenManager.PickChoice(user, message))
            {
                user.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("janken.choice", session.Langue));
            }

            return;
        }
        if (!session.User.HasPermission("mod") && !user.IsOwner() && !room.CheckRights(session) && room.UserIsMuted(session.User.Id))
        {
            if (!room.HasMuteExpired(session.User.Id))
            {
                user.Client?.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("user.muted", session.Langue));
                return;
            }
            else
            {
                room.RemoveMute(session.User.Id);
            }
        }

        var timeSpan = DateTime.Now - session.User.SpamFloodTime;
        if (timeSpan.TotalSeconds > session.User.SpamProtectionTime && session.User.SpamEnable)
        {
            session.User.FloodCount = 0;
            session.User.SpamEnable = false;
        }
        else if (timeSpan.TotalSeconds > 4.0)
        {
            session.User.FloodCount = 0;
        }

        if (timeSpan.TotalSeconds < session.User.SpamProtectionTime && session.User.SpamEnable)
        {
            var i = session.User.SpamProtectionTime - timeSpan.Seconds;
            user.Client?.SendPacket(new FloodControlComposer(i));
            return;
        }
        else if (timeSpan.TotalSeconds < 4.0 && session.User.FloodCount > 5 && !session.User.HasPermission("flood_chat"))
        {
            session.User.SpamProtectionTime = room.IsRoleplay || session.User.HasPermission("flood_premium") ? 5 : 15;
            session.User.SpamEnable = true;

            user.Client?.SendPacket(new FloodControlComposer(session.User.SpamProtectionTime - timeSpan.Seconds));

            return;
        }
        else if (message.Length > 40 && message == user.LastMessage && user.LastMessageCount == 1)
        {
            user.LastMessageCount = 0;
            user.LastMessage = "";

            session.User.SpamProtectionTime = room.IsRoleplay || session.User.HasPermission("flood_premium") ? 5 : 15;
            session.User.SpamEnable = true;
            user.Client?.SendPacket(new FloodControlComposer(session.User.SpamProtectionTime - timeSpan.Seconds));
            return;
        }
        else
        {
            if (message == user.LastMessage && message.Length > 40)
            {
                user.LastMessageCount++;
            }

            user.LastMessage = message;

            session.User.SpamFloodTime = DateTime.Now;
            session.User.FloodCount++;

            if (message.StartsWith("@red@"))
            {
                user.ChatTextColor = "@red@";
            }

            if (message.StartsWith("@cyan@"))
            {
                user.ChatTextColor = "@cyan@";
            }

            if (message.StartsWith("@blue@"))
            {
                user.ChatTextColor = "@blue@";
            }

            if (message.StartsWith("@green@"))
            {
                user.ChatTextColor = "@green@";
            }

            if (message.StartsWith("@purple@"))
            {
                user.ChatTextColor = "@purple@";
            }

            if (message.StartsWith("@black@"))
            {
                user.ChatTextColor = "";
            }

            if (message.StartsWith(":", StringComparison.CurrentCulture) && WibboEnvironment.GetGame().GetChatManager().GetCommands().Parse(session, user, room, message))
            {
                room.ChatlogManager.AddMessage(session.User.Id, session.User.Username, room.Id, string.Format("{0} a utilisé la commande {1}", session.User.Username, message), UnixTimestamp.GetNow());
                return;
            }

            if (!user.IsBot)
            {
                if (session.User.Antipub(message, "<TCHAT>", room.Id))
                {
                    return;
                }

                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.SocialChat, 0);
                session.User.ChatMessageManager.AddMessage(session.User.Id, session.User.Username, room.Id, message, UnixTimestamp.GetNow());
                room.ChatlogManager.AddMessage(session.User.Id, session.User.Username, room.Id, message, UnixTimestamp.GetNow());

                if (user.TransfBot)
                {
                    color = 2;
                }
            }
        }

        if (!session.User.HasPermission("word_filter_override"))
        {
            message = WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(message);
        }

        message = MyRegex().Replace(message, "<tag>$1</tag>");

        if (room.AllowsShous(user, message))
        {
            user.SendWhisperChat(message, true);
            return;
        }

        room.OnUserSay(user, message, true);

        if (user.IsSpectator)
        {
            return;
        }

        if (!session.User.IgnoreAll)
        {
            message = WibboEnvironment.GetGame().GetChatManager().GetMention().Parse(session, message);
        }

        if (!string.IsNullOrEmpty(user.ChatTextColor))
        {
            message = user.ChatTextColor + " " + message;
        }

        user.OnChat(message, color, true);
    }

    [GeneratedRegex("\\[tag\\](.*?)\\[\\/tag\\]")]
    private static partial Regex MyRegex();
}
