namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Chat;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Utilities;

internal class WhisperEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.GetUser() == null)
        {
            return;
        }

        var room = session.GetUser().CurrentRoom;
        if (room == null)
        {
            return;
        }

        var parameters = StringCharFilter.Escape(packet.PopString());
        if (string.IsNullOrEmpty(parameters) || parameters.Length > 100 || !parameters.Contains(' '))
        {
            return;
        }

        var toUser = parameters.Split(new char[1] { ' ' })[0];

        if (toUser.Length + 1 > parameters.Length)
        {
            return;
        }

        var message = parameters[(toUser.Length + 1)..];
        var color = packet.PopInt();

        if (!WibboEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(color, out var style) || (style.RequiredRight.Length > 0 && !session.GetUser().HasPermission(style.RequiredRight)))
        {
            color = 0;
        }

        if (session.Antipub(message, "<MP>"))
        {
            return;
        }

        if (!session.GetUser().HasPermission("perm_word_filter_override"))
        {
            message = WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(message);
        }

        var user = room.RoomUserManager.GetRoomUserByUserId(session.GetUser().Id);

        if (user == null)
        {
            return;
        }

        if (!session.GetUser().HasPermission("perm_mod") && !user.IsOwner() && !session.GetUser().CurrentRoom.CheckRights(session) && room.UserIsMuted(session.GetUser().Id))
        {
            if (!room.HasMuteExpired(session.GetUser().Id))
            {
                return;
            }
            else
            {
                room.RemoveMute(session.GetUser().Id);
            }
        }

        if (user.IsSpectator)
        {
            return;
        }

        var timeSpan = DateTime.Now - session.GetUser().SpamFloodTime;
        if (timeSpan.TotalSeconds > session.GetUser().SpamProtectionTime && session.GetUser().SpamEnable)
        {
            session.GetUser().FloodCount = 0;
            session.GetUser().SpamEnable = false;
        }
        else if (timeSpan.TotalSeconds > 4.0)
        {
            session.GetUser().FloodCount = 0;
        }

        if (timeSpan.TotalSeconds < session.GetUser().SpamProtectionTime && session.GetUser().SpamEnable)
        {
            var floodSeconds = session.GetUser().SpamProtectionTime - timeSpan.Seconds;
            session.GetUser().Client.SendPacket(new FloodControlComposer(floodSeconds));
            return;
        }
        else if (timeSpan.TotalSeconds < 4.0 && session.GetUser().FloodCount > 5 && !session.GetUser().HasPermission("perm_mod"))
        {
            session.GetUser().SpamProtectionTime = room.IsRoleplay || session.GetUser().HasPermission("perm_flood_premium") ? 5 : 15;
            session.GetUser().SpamEnable = true;

            user.Client.SendPacket(new FloodControlComposer(session.GetUser().SpamProtectionTime - timeSpan.Seconds));

            return;
        }
        else if (message.Length > 40 && message == user.LastMessage && user.LastMessageCount == 1)
        {
            user.LastMessageCount = 0;
            user.LastMessage = "";

            session.GetUser().SpamProtectionTime = room.IsRoleplay || session.GetUser().HasPermission("perm_flood_premium") ? 5 : 15;
            session.GetUser().SpamEnable = true;
            user.Client.SendPacket(new FloodControlComposer(session.GetUser().SpamProtectionTime - timeSpan.Seconds));
            return;
        }
        else
        {
            if (message == user.LastMessage && message.Length > 40)
            {
                user.LastMessageCount++;
            }

            user.LastMessage = message;

            session.GetUser().SpamFloodTime = DateTime.Now;
            session.GetUser().FloodCount++;

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

            if (!string.IsNullOrEmpty(user.ChatTextColor))
            {
                message = user.ChatTextColor + " " + message;
            }

            user.Unidle();

            if (toUser == "groupe")
            {
                if (user.WhiperGroupUsers.Count <= 0)
                {
                    return;
                }

                var groupUsername = string.Join(", ", user.WhiperGroupUsers);

                message = "(" + groupUsername + ") " + message;

                user.Client.SendPacket(new WhisperComposer(user.VirtualId, message, color));

                if (session.GetUser().IgnoreAll)
                {
                    return;
                }

                foreach (var username in user.WhiperGroupUsers.ToArray())
                {
                    var userWhiper = room.RoomUserManager.GetRoomUserByName(username);

                    if (userWhiper == null || userWhiper.Client == null || userWhiper.Client.GetUser() == null)
                    {
                        _ = user.WhiperGroupUsers.Remove(username);
                        continue;
                    }

                    if (userWhiper.IsSpectator || userWhiper.IsBot || userWhiper.UserId == user.UserId || userWhiper.Client.GetUser().MutedUsers.Contains(session.GetUser().Id))
                    {
                        _ = user.WhiperGroupUsers.Remove(username);
                        continue;
                    }

                    userWhiper.Client.SendPacket(new WhisperComposer(user.VirtualId, message, color));
                }

                var roomUserByRank = room.RoomUserManager.GetStaffRoomUser();
                if (roomUserByRank.Count <= 0)
                {
                    return;
                }

                var messageWhipser = new WhisperComposer(user.VirtualId, WibboEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", session.Langue) + toUser + ": " + message, color);

                foreach (var roomUser in roomUserByRank)
                {
                    if (roomUser != null && roomUser.UserId != user.UserId && roomUser.Client != null && roomUser.Client.GetUser().ViewMurmur && !user.WhiperGroupUsers.Contains(roomUser.GetUsername()))
                    {
                        roomUser.Client.SendPacket(messageWhipser);
                    }
                }
            }
            else
            {
                user.Client.SendPacket(new WhisperComposer(user.VirtualId, message, color));

                if (session.GetUser().IgnoreAll)
                {
                    return;
                }

                var userWhiper = room.RoomUserManager.GetRoomUserByName(toUser);

                if (userWhiper == null || userWhiper.Client == null || userWhiper.Client.GetUser() == null)
                {
                    return;
                }

                if (userWhiper.IsSpectator || userWhiper.IsBot || userWhiper.UserId == user.UserId || userWhiper.Client.GetUser().MutedUsers.Contains(session.GetUser().Id))
                {
                    return;
                }

                userWhiper.Client.SendPacket(new WhisperComposer(user.VirtualId, message, color));

                var roomUserByRank = room.RoomUserManager.GetStaffRoomUser();
                if (roomUserByRank.Count <= 0)
                {
                    return;
                }

                var messageWhipserStaff = new WhisperComposer(user.VirtualId, WibboEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", session.Langue) + toUser + ": " + message, color);
                foreach (var roomUser in roomUserByRank)
                {
                    if (roomUser != null && roomUser.Client != null && roomUser.Client.GetUser() != null && roomUser.UserId != user.UserId && roomUser.Client != null && roomUser.Client.GetUser().ViewMurmur && userWhiper.UserId != roomUser.UserId)
                    {
                        roomUser.Client.SendPacket(messageWhipserStaff);
                    }
                }
            }

            session.GetUser().
            ChatMessageManager.AddMessage(user.UserId, user.GetUsername(), user.RoomId, WibboEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", session.Langue) + toUser + ": " + message, UnixTimestamp.GetNow());
            room.ChatlogManager.AddMessage(user.UserId, user.GetUsername(), user.RoomId, WibboEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", session.Langue) + toUser + ": " + message, UnixTimestamp.GetNow());
        }
    }
}
