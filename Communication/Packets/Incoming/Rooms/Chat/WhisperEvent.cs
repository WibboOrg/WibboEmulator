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

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null)
        {
            return;
        }

        var room = session.User.Room;
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

        if (!ChatStyleManager.TryGetStyle(color, out var style) || (style.RequiredRight.Length > 0 && !session.User.HasPermission(style.RequiredRight)))
        {
            color = 0;
        }

        if (color == 23)
        {
            color = session.User.BadgeComponent.StaffBulleId;
        }

        if (session.User.CheckChatMessage(message, "<MP>", room.Id))
        {
            return;
        }

        if (!session.User.HasPermission("word_filter_override"))
        {
            message = WordFilterManager.CheckMessage(message);
        }

        var user = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);

        if (user == null)
        {
            return;
        }

        if (!session.User.HasPermission("mod") && !user.IsOwner && !room.CheckRights(session) && room.UserIsMuted(session.User.Id))
        {
            if (!room.HasMuteExpired(session.User.Id))
            {
                return;
            }
            else
            {
                room.RemoveMute(session.User.Id);
            }
        }

        if (user.IsSpectator)
        {
            return;
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
            var floodSeconds = session.User.SpamProtectionTime - timeSpan.Seconds;
            session.User.Client.SendPacket(new FloodControlComposer(floodSeconds));
            return;
        }
        else if (timeSpan.TotalSeconds < 4.0 && session.User.FloodCount > 5 && !session.User.HasPermission("mod"))
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

                if (session.User.IgnoreAll)
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

                    if (userWhiper.IsSpectator || userWhiper.IsBot || userWhiper.UserId == user.UserId || userWhiper.Client.User.MutedUsers.Contains(session.User.Id))
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

                var messageWhipser = new WhisperComposer(user.VirtualId, LanguageManager.TryGetValue("moderation.whisper", session.Language) + toUser + ": " + message, color);

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

                if (session.User.IgnoreAll)
                {
                    return;
                }

                var userWhiper = room.RoomUserManager.GetRoomUserByName(toUser);

                if (userWhiper == null || userWhiper.Client == null || userWhiper.Client.User == null)
                {
                    return;
                }

                if (userWhiper.IsSpectator || userWhiper.IsBot || userWhiper.UserId == user.UserId || userWhiper.Client.User.MutedUsers.Contains(session.User.Id))
                {
                    return;
                }

                userWhiper.Client.SendPacket(new WhisperComposer(user.VirtualId, message, color));

                var roomUserByRank = room.RoomUserManager.StaffRoomUsers;
                if (roomUserByRank.Count <= 0)
                {
                    return;
                }

                var messageWhipserStaff = new WhisperComposer(user.VirtualId, LanguageManager.TryGetValue("moderation.whisper", session.Language) + toUser + ": " + message, color);
                foreach (var roomUser in roomUserByRank)
                {
                    if (roomUser != null && roomUser.Client != null && roomUser.Client.User != null && roomUser.UserId != user.UserId && roomUser.Client != null && roomUser.Client.User.ViewMurmur && userWhiper.UserId != roomUser.UserId)
                    {
                        roomUser.Client.SendPacket(messageWhipserStaff);
                    }
                }
            }

            session.User.ChatMessageManager.AddMessage(user.UserId, user.Username, user.RoomId, LanguageManager.TryGetValue("moderation.whisper", session.Language) + toUser + ": " + message, UnixTimestamp.GetNow());
            room.ChatlogManager.AddMessage(user.UserId, user.Username, user.RoomId, LanguageManager.TryGetValue("moderation.whisper", session.Language) + toUser + ": " + message, UnixTimestamp.GetNow());
        }
    }
}
