namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Chat;
using System.Text.RegularExpressions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.Chats.Commands;
using WibboEmulator.Games.Chats.Filter;
using WibboEmulator.Games.Chats.Mentions;
using WibboEmulator.Games.Chats.Styles;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;
using WibboEmulator.Utilities;

internal sealed partial class ChatEvent(bool isShout = false) : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null || !Session.User.InRoom)
        {
            return;
        }

        var room = Session.User.Room;
        if (room == null)
        {
            return;
        }

        var user = room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
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

        var message = packet.PopString(100);
        var color = packet.PopInt();
        var chatColour = packet.PopString(10);

        if (!ChatStyleManager.TryGetStyle(color, out var style) || (style.RequiredRight.Length > 0 && !Session.User.HasPermission(style.RequiredRight)))
        {
            color = 0;
        }

        if (color == 23)
        {
            color = Session.User.BadgeComponent.StaffBulleId;
        }
        else
        {
            message = StringCharFilter.Escape(message);
        }

        user.Unidle();

        if (!Session.User.HasPermission("no_mute") && !user.IsOwner && !room.CheckRights(Session) && room.RoomMuted)
        {
            user.SendWhisperChat(LanguageManager.TryGetValue("room.muted", Session.Language));
            return;
        }

        if (room.JankenManager.PlayerStarted(user))
        {
            if (!room.JankenManager.PickChoice(user, message))
            {
                user.SendWhisperChat(LanguageManager.TryGetValue("janken.choice", Session.Language));
            }

            return;
        }

        if (!Session.User.HasPermission("mod") && !user.IsOwner && !room.CheckRights(Session) && room.UserIsMuted(Session.User.Id))
        {
            if (!room.HasMuteExpired(Session.User.Id))
            {
                user.SendWhisperChat(LanguageManager.TryGetValue("user.muted", Session.Language));
                return;
            }
            else
            {
                room.RemoveMute(Session.User.Id);
            }
        }

        var timeSpan = DateTime.Now - Session.User.SpamFloodTime;
        if (timeSpan.TotalSeconds > Session.User.SpamProtectionTime && Session.User.SpamEnable)
        {
            Session.User.FloodCount = 0;
            Session.User.SpamEnable = false;
        }
        else if (timeSpan.TotalSeconds > 4.0)
        {
            Session.User.FloodCount = 0;
        }

        if (timeSpan.TotalSeconds < Session.User.SpamProtectionTime && Session.User.SpamEnable)
        {
            var i = Session.User.SpamProtectionTime - timeSpan.Seconds;
            user.Client?.SendPacket(new FloodControlComposer(i));
            return;
        }
        else if (timeSpan.TotalSeconds < 4.0 && Session.User.FloodCount > 5 && !Session.User.HasPermission("flood_chat"))
        {
            Session.User.SpamProtectionTime = room.IsRoleplay || Session.User.HasPermission("flood_premium") ? 5 : 15;
            Session.User.SpamEnable = true;

            user.Client?.SendPacket(new FloodControlComposer(Session.User.SpamProtectionTime - timeSpan.Seconds));

            return;
        }
        else if (message.Length > 40 && message == user.LastMessage && user.LastMessageCount == 1)
        {
            user.LastMessageCount = 0;
            user.LastMessage = "";

            Session.User.SpamProtectionTime = room.IsRoleplay || Session.User.HasPermission("flood_premium") ? 5 : 15;
            Session.User.SpamEnable = true;
            user.Client?.SendPacket(new FloodControlComposer(Session.User.SpamProtectionTime - timeSpan.Seconds));
            return;
        }

        if (message == user.LastMessage && message.Length > 40)
        {
            user.LastMessageCount++;
        }

        user.LastMessage = message;

        Session.User.SpamFloodTime = DateTime.Now;
        Session.User.FloodCount++;

        if (message.StartsWith(":", StringComparison.CurrentCulture) && CommandManager.Parse(Session, user, room, message))
        {
            room.ChatlogManager.AddMessage(Session.User.Id, Session.User.Username, room.Id, string.Format("{0} a utilisé la commande {1}", Session.User.Username, message), UnixTimestamp.GetNow());
            return;
        }

        if (Session.User.CheckChatMessage(message, "<TCHAT>", room.Id))
        {
            return;
        }

        QuestManager.ProgressUserQuest(Session, QuestType.SocialChat, 0);
        Session.User.ChatMessageManager.AddMessage(Session.User.Id, Session.User.Username, room.Id, message, UnixTimestamp.GetNow());
        room.ChatlogManager.AddMessage(Session.User.Id, Session.User.Username, room.Id, message, UnixTimestamp.GetNow());

        if (user.TransfBot)
        {
            color = 2;
        }

        if (!Session.User.HasPermission("word_filter_override"))
        {
            message = WordFilterManager.CheckMessage(message);
        }

        message = MyRegex().Replace(message, "<tag>$1</tag>");

        if (room.AllowsShous(user, message))
        {
            user.SendWhisperChat(message, false);
            return;
        }

        if (message.StartsWith(":", StringComparison.CurrentCulture) && room.OnCommand(user, message))
        {
            return;
        }

        room.OnUserSay(user, message, isShout);

        if (user.IsSpectator)
        {
            return;
        }

        if (!Session.User.IgnoreAll)
        {
            message = MentionManager.Parse(Session, message);
        }

        user.OnChat(message, color, isShout, chatColour);
    }

    [GeneratedRegex("\\[tag\\](.*?)\\[\\/tag\\]")]
    private static partial Regex MyRegex();
}
