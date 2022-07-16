using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;

using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Chat.Styles;
using WibboEmulator.Utilities;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class WhisperEvent : IPacketEvent
    {
        public double Delay => 100;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null)
            {
                return;
            }

            Room Room = Session.GetUser().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            if (Room.UserIsMuted(Session.GetUser().Id))
            {
                if (!Room.HasMuteExpired(Session.GetUser().Id))
                {
                    return;
                }
                else
                {
                    Room.RemoveMute(Session.GetUser().Id);
                }
            }

            string Params = StringCharFilter.Escape(Packet.PopString());
            if (string.IsNullOrEmpty(Params) || Params.Length > 100 || !Params.Contains(' '))
            {
                return;
            }

            string ToUser = Params.Split(new char[1] { ' ' })[0];

            if (ToUser.Length + 1 > Params.Length)
            {
                return;
            }

            string Message = Params.Substring(ToUser.Length + 1);
            int Color = Packet.PopInt();

            if (!WibboEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(Color, out ChatStyle Style) || (Style.RequiredRight.Length > 0 && !Session.GetUser().HasPermission(Style.RequiredRight)))
            {
                Color = 0;
            }

            if (Session.Antipub(Message, "<MP>"))
            {
                return;
            }

            if (!Session.GetUser().HasPermission("word_filter_override"))
            {
                Message = WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Message);
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);

            if (User == null)
            {
                return;
            }

            if (User.IsSpectator)
            {
                return;
            }

            TimeSpan timeSpan = DateTime.Now - Session.GetUser().SpamFloodTime;
            if (timeSpan.TotalSeconds > Session.GetUser().SpamProtectionTime && Session.GetUser().SpamEnable)
            {
                User.FloodCount = 0;
                Session.GetUser().SpamEnable = false;
            }
            else if (timeSpan.TotalSeconds > 4.0)
            {
                User.FloodCount = 0;
            }

            if (timeSpan.TotalSeconds < Session.GetUser().SpamProtectionTime && Session.GetUser().SpamEnable)
            {
                int floodSeconds = Session.GetUser().SpamProtectionTime - timeSpan.Seconds;
                User.GetClient().SendPacket(new FloodControlComposer(floodSeconds));
                return;
            }
            else if (timeSpan.TotalSeconds < 4.0 && User.FloodCount > 5 && !Session.GetUser().HasPermission("perm_mod"))
            {
                Session.GetUser().SpamProtectionTime = (Room.IsRoleplay || Session.GetUser().HasPermission("fuse_low_flood")) ? 5 : 15;
                Session.GetUser().SpamEnable = true;

                User.GetClient().SendPacket(new FloodControlComposer(Session.GetUser().SpamProtectionTime - timeSpan.Seconds));

                return;
            }
            else if (Message.Length > 40 && Message == User.LastMessage && User.LastMessageCount == 1)
            {
                User.LastMessageCount = 0;
                User.LastMessage = "";

                Session.GetUser().SpamProtectionTime = (Room.IsRoleplay || Session.GetUser().HasPermission("fuse_low_flood")) ? 5 : 15;
                Session.GetUser().SpamEnable = true;
                User.GetClient().SendPacket(new FloodControlComposer(Session.GetUser().SpamProtectionTime - timeSpan.Seconds));
                return;
            }
            else
            {
                if (Message == User.LastMessage && Message.Length > 40)
                {
                    User.LastMessageCount++;
                }

                User.LastMessage = Message;

                Session.GetUser().SpamFloodTime = DateTime.Now;
                User.FloodCount++;

                if (Message.StartsWith("@red@"))
                {
                    User.ChatTextColor = "@red@";
                }

                if (Message.StartsWith("@cyan@"))
                {
                    User.ChatTextColor = "@cyan@";
                }

                if (Message.StartsWith("@blue@"))
                {
                    User.ChatTextColor = "@blue@";
                }

                if (Message.StartsWith("@green@"))
                {
                    User.ChatTextColor = "@green@";
                }

                if (Message.StartsWith("@purple@"))
                {
                    User.ChatTextColor = "@purple@";
                }

                if (Message.StartsWith("@black@"))
                {
                    User.ChatTextColor = "";
                }

                if (!string.IsNullOrEmpty(User.ChatTextColor))
                {
                    Message = User.ChatTextColor + " " + Message;
                }

                User.Unidle();

                if (ToUser == "groupe")
                {
                    if (User.WhiperGroupUsers.Count <= 0)
                    {
                        return;
                    }

                    string GroupUsername = string.Join(", ", User.WhiperGroupUsers);

                    Message = "(" + GroupUsername + ") " + Message;

                    User.GetClient().SendPacket(new WhisperComposer(User.VirtualId, Message, Color));

                    if (Session.GetUser().IgnoreAll)
                    {
                        return;
                    }

                    foreach (string Username in User.WhiperGroupUsers.ToArray())
                    {
                        RoomUser UserWhiper = Room.GetRoomUserManager().GetRoomUserByName(Username);

                        if (UserWhiper == null || UserWhiper.GetClient() == null || UserWhiper.GetClient().GetUser() == null)
                        {
                            User.WhiperGroupUsers.Remove(Username);
                            continue;
                        }

                        if (UserWhiper.IsSpectator || UserWhiper.IsBot || UserWhiper.UserId == User.UserId || UserWhiper.GetClient().GetUser().MutedUsers.Contains(Session.GetUser().Id))
                        {
                            User.WhiperGroupUsers.Remove(Username);
                            continue;
                        }

                        UserWhiper.GetClient().SendPacket(new WhisperComposer(User.VirtualId, Message, Color));
                    }

                    List<RoomUser> roomUserByRank = Room.GetRoomUserManager().GetStaffRoomUser();
                    if (roomUserByRank.Count <= 0)
                    {
                        return;
                    }

                    WhisperComposer MessageWhipser = new WhisperComposer(User.VirtualId, WibboEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", Session.Langue) + ToUser + ": " + Message, Color);
               
                    foreach (RoomUser roomUser in roomUserByRank)
                    {
                        if (roomUser != null && roomUser.UserId != User.UserId && roomUser.GetClient() != null && roomUser.GetClient().GetUser().ViewMurmur && !User.WhiperGroupUsers.Contains(roomUser.GetUsername()))
                        {
                            roomUser.GetClient().SendPacket(MessageWhipser);
                        }
                    }
                }
                else
                {
                    User.GetClient().SendPacket(new WhisperComposer(User.VirtualId, Message, Color));

                    if (Session.GetUser().IgnoreAll)
                    {
                        return;
                    }

                    RoomUser UserWhiper = Room.GetRoomUserManager().GetRoomUserByName(ToUser);

                    if (UserWhiper == null || UserWhiper.GetClient() == null || UserWhiper.GetClient().GetUser() == null)
                    {
                        return;
                    }

                    if (UserWhiper.IsSpectator || UserWhiper.IsBot || UserWhiper.UserId == User.UserId || UserWhiper.GetClient().GetUser().MutedUsers.Contains(Session.GetUser().Id))
                    {
                        return;
                    }

                    UserWhiper.GetClient().SendPacket(new WhisperComposer(User.VirtualId, Message, Color));

                    List<RoomUser> roomUserByRank = Room.GetRoomUserManager().GetStaffRoomUser();
                    if (roomUserByRank.Count <= 0)
                    {
                        return;
                    }

                    WhisperComposer MessageWhipserStaff = new WhisperComposer(User.VirtualId, WibboEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", Session.Langue) + ToUser + ": " + Message, Color);
                    foreach (RoomUser roomUser in roomUserByRank)
                    {
                        if (roomUser != null && roomUser.GetClient() != null && roomUser.GetClient().GetUser() != null && roomUser.UserId != User.UserId && roomUser.GetClient() != null && roomUser.GetClient().GetUser().ViewMurmur && UserWhiper.UserId != roomUser.UserId)
                        {
                            roomUser.GetClient().SendPacket(MessageWhipserStaff);
                        }
                    }
                }

                Session.GetUser().GetChatMessageManager().AddMessage(User.UserId, User.GetUsername(), User.RoomId, WibboEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", Session.Langue) + ToUser + ": " + Message, UnixTimestamp.GetNow());
                Room.GetChatMessageManager().AddMessage(User.UserId, User.GetUsername(), User.RoomId, WibboEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", Session.Langue) + ToUser + ": " + Message, UnixTimestamp.GetNow());
            }
        }
    }
}
