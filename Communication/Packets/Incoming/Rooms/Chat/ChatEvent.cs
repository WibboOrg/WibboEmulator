namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Chat;
using System.Text.RegularExpressions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Communication.RCON.Commands;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.Chats.Commands;
using WibboEmulator.Games.Chats.Filter;
using WibboEmulator.Games.Chats.Mentions;
using WibboEmulator.Games.Chats.Styles;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;
using WibboEmulator.Utilities;

internal sealed partial class ChatEvent : IPacketEvent
{
    public double Delay => 100;
    private readonly bool _isShout;

    public ChatEvent(bool isShout = false) => this._isShout = isShout;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null || !session.User.InRoom)
        {
            return;
        }

        var room = session.User.Room;
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

        var message = packet.PopString(100);
        var color = packet.PopInt();
        var chatColour = packet.PopString(10);

        if (!ChatStyleManager.TryGetStyle(color, out var style) || (style.RequiredRight.Length > 0 && !session.User.HasPermission(style.RequiredRight)))
        {
            color = 0;
        }

        if (color == 23)
        {
            color = session.User.BadgeComponent.StaffBulleId;
        }
        else
        {
            message = StringCharFilter.Escape(message);
        }

        user.Unidle();

        if (!session.User.HasPermission("no_mute") && !user.IsOwner && !room.CheckRights(session) && room.RoomMuted)
        {
            user.SendWhisperChat(LanguageManager.TryGetValue("room.muted", session.Language));
            return;
        }

        if (room.JankenManager.PlayerStarted(user))
        {
            if (!room.JankenManager.PickChoice(user, message))
            {
                user.SendWhisperChat(LanguageManager.TryGetValue("janken.choice", session.Language));
            }

            return;
        }

        if (!session.User.HasPermission("mod") && !user.IsOwner && !room.CheckRights(session) && room.UserIsMuted(session.User.Id))
        {
            if (!room.HasMuteExpired(session.User.Id))
            {
                user.SendWhisperChat(LanguageManager.TryGetValue("user.muted", session.Language));
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

        if (message == user.LastMessage && message.Length > 40)
        {
            user.LastMessageCount++;
        }

        user.LastMessage = message;

        session.User.SpamFloodTime = DateTime.Now;
        session.User.FloodCount++;

        if (message.StartsWith(":", StringComparison.CurrentCulture) && CommandManager.Parse(session, user, room, message))
        {
            room.ChatlogManager.AddMessage(session.User.Id, session.User.Username, room.Id, string.Format("{0} a utilisé la commande {1}", session.User.Username, message), UnixTimestamp.GetNow());
            return;
        }

        if (session.User.CheckChatMessage(message, "<TCHAT>", room.Id))
        {
            return;
        }

        QuestManager.ProgressUserQuest(session, QuestType.SocialChat, 0);
        session.User.ChatMessageManager.AddMessage(session.User.Id, session.User.Username, room.Id, message, UnixTimestamp.GetNow());
        room.ChatlogManager.AddMessage(session.User.Id, session.User.Username, room.Id, message, UnixTimestamp.GetNow());

        if (user.TransfBot)
        {
            color = 2;
        }

        if (!session.User.HasPermission("word_filter_override"))
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

        room.OnUserSay(user, message, this._isShout);

        if (user.IsSpectator)
        {
            return;
        }

        if (!session.User.IgnoreAll)
        {
            message = MentionManager.Parse(session, message);
        }

        user.OnChat(message, color, this._isShout, chatColour);
    }

    [GeneratedRegex("\\[tag\\](.*?)\\[\\/tag\\]")]
    private static partial Regex MyRegex();
}
