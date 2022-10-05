namespace WibboEmulator.Communication.Packets.Incoming.Structure;
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

        var Params = StringCharFilter.Escape(packet.PopString());
        if (string.IsNullOrEmpty(Params) || Params.Length > 100 || !Params.Contains(' '))
        {
            return;
        }

        var ToUser = Params.Split(new char[1] { ' ' })[0];

        if (ToUser.Length + 1 > Params.Length)
        {
            return;
        }

        var Message = Params[(ToUser.Length + 1)..];
        var Color = packet.PopInt();

        if (!WibboEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(Color, out var Style) || (Style.RequiredRight.Length > 0 && !session.GetUser().HasPermission(Style.RequiredRight)))
        {
            Color = 0;
        }

        if (session.Antipub(Message, "<MP>"))
        {
            return;
        }

        if (!session.GetUser().HasPermission("perm_word_filter_override"))
        {
            Message = WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Message);
        }

        var user = room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);

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
            session.GetUser().GetClient().SendPacket(new FloodControlComposer(floodSeconds));
            return;
        }
        else if (timeSpan.TotalSeconds < 4.0 && session.GetUser().FloodCount > 5 && !session.GetUser().HasPermission("perm_mod"))
        {
            session.GetUser().SpamProtectionTime = (room.IsRoleplay || session.GetUser().HasPermission("perm_flood_premium")) ? 5 : 15;
            session.GetUser().SpamEnable = true;

            user.GetClient().SendPacket(new FloodControlComposer(session.GetUser().SpamProtectionTime - timeSpan.Seconds));

            return;
        }
        else if (Message.Length > 40 && Message == user.LastMessage && user.LastMessageCount == 1)
        {
            user.LastMessageCount = 0;
            user.LastMessage = "";

            session.GetUser().SpamProtectionTime = (room.IsRoleplay || session.GetUser().HasPermission("perm_flood_premium")) ? 5 : 15;
            session.GetUser().SpamEnable = true;
            user.GetClient().SendPacket(new FloodControlComposer(session.GetUser().SpamProtectionTime - timeSpan.Seconds));
            return;
        }
        else
        {
            if (Message == user.LastMessage && Message.Length > 40)
            {
                user.LastMessageCount++;
            }

            user.LastMessage = Message;

            session.GetUser().SpamFloodTime = DateTime.Now;
            session.GetUser().FloodCount++;

            if (Message.StartsWith("@red@"))
            {
                user.ChatTextColor = "@red@";
            }

            if (Message.StartsWith("@cyan@"))
            {
                user.ChatTextColor = "@cyan@";
            }

            if (Message.StartsWith("@blue@"))
            {
                user.ChatTextColor = "@blue@";
            }

            if (Message.StartsWith("@green@"))
            {
                user.ChatTextColor = "@green@";
            }

            if (Message.StartsWith("@purple@"))
            {
                user.ChatTextColor = "@purple@";
            }

            if (Message.StartsWith("@black@"))
            {
                user.ChatTextColor = "";
            }

            if (!string.IsNullOrEmpty(user.ChatTextColor))
            {
                Message = user.ChatTextColor + " " + Message;
            }

            user.Unidle();

            if (ToUser == "groupe")
            {
                if (user.WhiperGroupUsers.Count <= 0)
                {
                    return;
                }

                var GroupUsername = string.Join(", ", user.WhiperGroupUsers);

                Message = "(" + GroupUsername + ") " + Message;

                user.GetClient().SendPacket(new WhisperComposer(user.VirtualId, Message, Color));

                if (session.GetUser().IgnoreAll)
                {
                    return;
                }

                foreach (var Username in user.WhiperGroupUsers.ToArray())
                {
                    var UserWhiper = room.GetRoomUserManager().GetRoomUserByName(Username);

                    if (UserWhiper == null || UserWhiper.GetClient() == null || UserWhiper.GetClient().GetUser() == null)
                    {
                        user.WhiperGroupUsers.Remove(Username);
                        continue;
                    }

                    if (UserWhiper.IsSpectator || UserWhiper.IsBot || UserWhiper.UserId == user.UserId || UserWhiper.GetClient().GetUser().MutedUsers.Contains(session.GetUser().Id))
                    {
                        user.WhiperGroupUsers.Remove(Username);
                        continue;
                    }

                    UserWhiper.GetClient().SendPacket(new WhisperComposer(user.VirtualId, Message, Color));
                }

                var roomUserByRank = room.GetRoomUserManager().GetStaffRoomUser();
                if (roomUserByRank.Count <= 0)
                {
                    return;
                }

                var MessageWhipser = new WhisperComposer(user.VirtualId, WibboEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", session.Langue) + ToUser + ": " + Message, Color);

                foreach (var roomUser in roomUserByRank)
                {
                    if (roomUser != null && roomUser.UserId != user.UserId && roomUser.GetClient() != null && roomUser.GetClient().GetUser().ViewMurmur && !user.WhiperGroupUsers.Contains(roomUser.GetUsername()))
                    {
                        roomUser.GetClient().SendPacket(MessageWhipser);
                    }
                }
            }
            else
            {
                user.GetClient().SendPacket(new WhisperComposer(user.VirtualId, Message, Color));

                if (session.GetUser().IgnoreAll)
                {
                    return;
                }

                var UserWhiper = room.GetRoomUserManager().GetRoomUserByName(ToUser);

                if (UserWhiper == null || UserWhiper.GetClient() == null || UserWhiper.GetClient().GetUser() == null)
                {
                    return;
                }

                if (UserWhiper.IsSpectator || UserWhiper.IsBot || UserWhiper.UserId == user.UserId || UserWhiper.GetClient().GetUser().MutedUsers.Contains(session.GetUser().Id))
                {
                    return;
                }

                UserWhiper.GetClient().SendPacket(new WhisperComposer(user.VirtualId, Message, Color));

                var roomUserByRank = room.GetRoomUserManager().GetStaffRoomUser();
                if (roomUserByRank.Count <= 0)
                {
                    return;
                }

                var MessageWhipserStaff = new WhisperComposer(user.VirtualId, WibboEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", session.Langue) + ToUser + ": " + Message, Color);
                foreach (var roomUser in roomUserByRank)
                {
                    if (roomUser != null && roomUser.GetClient() != null && roomUser.GetClient().GetUser() != null && roomUser.UserId != user.UserId && roomUser.GetClient() != null && roomUser.GetClient().GetUser().ViewMurmur && UserWhiper.UserId != roomUser.UserId)
                    {
                        roomUser.GetClient().SendPacket(MessageWhipserStaff);
                    }
                }
            }

            session.GetUser().GetChatMessageManager().AddMessage(user.UserId, user.GetUsername(), user.RoomId, WibboEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", session.Langue) + ToUser + ": " + Message, UnixTimestamp.GetNow());
            room.GetChatMessageManager().AddMessage(user.UserId, user.GetUsername(), user.RoomId, WibboEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", session.Langue) + ToUser + ": " + Message, UnixTimestamp.GetNow());
        }
    }
}
