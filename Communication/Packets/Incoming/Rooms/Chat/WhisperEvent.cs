namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Chat;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.Chats.Filter;
using WibboEmulator.Games.Chats.Styles;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Utilities;

internal sealed class WhisperEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null)
        {
            return;
        }

        var room = Session.User.Room;
        if (room == null)
        {
            return;
        }

        var parameters = packet.PopString(100);
        if (string.IsNullOrEmpty(parameters) || parameters.Length > 100 || !parameters.Contains(' '))
        {
            return;
        }

        var toUser = parameters.Split(' ')[0];

        if (toUser.Length + 1 > parameters.Length)
        {
            return;
        }

        var message = parameters[(toUser.Length + 1)..];
        var color = packet.PopInt();

        if (!ChatStyleManager.TryGetStyle(color, out var style) || (style.RequiredRight.Length > 0 && !Session.User.HasPermission(style.RequiredRight)))
        {
            color = 0;
        }

        if (color == 23)
        {
            color = Session.User.BadgeComponent.StaffBulleId;
        }

        if (Session.User.CheckChatMessage(message, "<MP>", room.Id))
        {
            return;
        }

        if (!Session.User.HasPermission("word_filter_override"))
        {
            message = WordFilterManager.CheckMessage(message);
        }

        var user = room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);

        if (user == null)
        {
            return;
        }

        if (!Session.User.HasPermission("mod") && !user.IsOwner && !room.CheckRights(Session) && room.UserIsMuted(Session.User.Id))
        {
            if (!room.HasMuteExpired(Session.User.Id))
            {
                return;
            }
            else
            {
                room.RemoveMute(Session.User.Id);
            }
        }

        if (user.IsSpectator)
        {
            return;
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
            var floodSeconds = Session.User.SpamProtectionTime - timeSpan.Seconds;
            Session.User.Client.SendPacket(new FloodControlComposer(floodSeconds));
            return;
        }
        else if (timeSpan.TotalSeconds < 4.0 && Session.User.FloodCount > 5 && !Session.User.HasPermission("mod"))
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
        else
        {
            if (message == user.LastMessage && message.Length > 40)
            {
                user.LastMessageCount++;
            }

            user.LastMessage = message;

            Session.User.SpamFloodTime = DateTime.Now;
            Session.User.FloodCount++;

            user.Unidle();

            if (toUser == "groupe")
            {
                if (user.WhiperGroupUsers.Count <= 0)
                {
                    return;
                }

                var groupUsername = string.Join(", ", user.WhiperGroupUsers);

                message = "(" + groupUsername + ") " + message;

                user.Client?.SendPacket(new WhisperComposer(user.VirtualId, message, color));

                if (Session.User.IgnoreAll)
                {
                    return;
                }

                foreach (var username in user.WhiperGroupUsers.ToArray())
                {
                    var userWhiper = room.RoomUserManager.GetRoomUserByName(username);

                    if (userWhiper == null || userWhiper.Client == null || userWhiper.Client.User == null)
                    {
                        _ = user.WhiperGroupUsers.Remove(username);
                        continue;
                    }

                    if (userWhiper.IsSpectator || userWhiper.IsBot || userWhiper.UserId == user.UserId || userWhiper.Client.User.MutedUsers.Contains(Session.User.Id))
                    {
                        _ = user.WhiperGroupUsers.Remove(username);
                        continue;
                    }

                    userWhiper.Client.SendPacket(new WhisperComposer(user.VirtualId, message, color));
                }

                var roomUserByRank = room.RoomUserManager.StaffRoomUsers;
                if (roomUserByRank.Count <= 0)
                {
                    return;
                }

                var messageWhipser = new WhisperComposer(user.VirtualId, LanguageManager.TryGetValue("moderation.whisper", Session.Language) + toUser + ": " + message, color);

                foreach (var roomUser in roomUserByRank)
                {
                    if (roomUser != null && roomUser.UserId != user.UserId && roomUser.Client != null && roomUser.Client.User.ViewMurmur && !user.WhiperGroupUsers.Contains(roomUser.Username))
                    {
                        roomUser.Client.SendPacket(messageWhipser);
                    }
                }
            }
            else
            {
                user.Client?.SendPacket(new WhisperComposer(user.VirtualId, message, color));

                if (Session.User.IgnoreAll)
                {
                    return;
                }

                var userWhiper = room.RoomUserManager.GetRoomUserByName(toUser);

                if (userWhiper == null || userWhiper.Client == null || userWhiper.Client.User == null)
                {
                    return;
                }

                if (userWhiper.IsSpectator || userWhiper.IsBot || userWhiper.UserId == user.UserId || userWhiper.Client.User.MutedUsers.Contains(Session.User.Id))
                {
                    return;
                }

                userWhiper.Client.SendPacket(new WhisperComposer(user.VirtualId, message, color));

                var roomUserByRank = room.RoomUserManager.StaffRoomUsers;
                if (roomUserByRank.Count <= 0)
                {
                    return;
                }

                var messageWhipserStaff = new WhisperComposer(user.VirtualId, LanguageManager.TryGetValue("moderation.whisper", Session.Language) + toUser + ": " + message, color);
                foreach (var roomUser in roomUserByRank)
                {
                    if (roomUser != null && roomUser.Client != null && roomUser.Client.User != null && roomUser.UserId != user.UserId && roomUser.Client != null && roomUser.Client.User.ViewMurmur && userWhiper.UserId != roomUser.UserId)
                    {
                        roomUser.Client.SendPacket(messageWhipserStaff);
                    }
                }
            }

            Session.User.ChatMessageManager.AddMessage(user.UserId, user.Username, user.RoomId, LanguageManager.TryGetValue("moderation.whisper", Session.Language) + toUser + ": " + message, UnixTimestamp.GetNow());
            room.ChatlogManager.AddMessage(user.UserId, user.Username, user.RoomId, LanguageManager.TryGetValue("moderation.whisper", Session.Language) + toUser + ": " + message, UnixTimestamp.GetNow());
        }
    }
}
