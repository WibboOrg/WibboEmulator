using Butterfly.Communication.Packets.Outgoing.Rooms.Chat;

using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using Butterfly.Game.Chat.Styles;
using Butterfly.Utilities;
using System;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class WhisperEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            if (Room.UserIsMuted(Session.GetHabbo().Id))
            {
                if (!Room.HasMuteExpired(Session.GetHabbo().Id))
                {
                    return;
                }
                else
                {
                    Room.RemoveMute(Session.GetHabbo().Id);
                }
            }

            string Params = StringCharFilter.Escape(Packet.PopString());
            if (string.IsNullOrEmpty(Params) || Params.Length > 100 || !Params.Contains(" "))
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

            if (!ButterflyEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(Color, out ChatStyle Style) || (Style.RequiredRight.Length > 0 && !Session.GetHabbo().HasFuse(Style.RequiredRight)))
            {
                Color = 0;
            }

            if (Session.Antipub(Message, "<MP>"))
            {
                return;
            }

            if (!Session.GetHabbo().HasFuse("word_filter_override"))
            {
                Message = ButterflyEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Message);
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);

            if (User == null)
            {
                return;
            }

            if (User.IsSpectator)
            {
                return;
            }

            TimeSpan timeSpan = DateTime.Now - Session.GetHabbo().SpamFloodTime;
            if (timeSpan.TotalSeconds > Session.GetHabbo().SpamProtectionTime && Session.GetHabbo().SpamEnable)
            {
                User.FloodCount = 0;
                Session.GetHabbo().SpamEnable = false;
            }
            else if (timeSpan.TotalSeconds > 4.0)
            {
                User.FloodCount = 0;
            }

            if (timeSpan.TotalSeconds < Session.GetHabbo().SpamProtectionTime && Session.GetHabbo().SpamEnable)
            {
                int i = Session.GetHabbo().SpamProtectionTime - timeSpan.Seconds;
                User.GetClient().SendPacket(new FloodControlComposer(i));
                return;
            }
            else if (timeSpan.TotalSeconds < 4.0 && User.FloodCount > 5 && !Session.GetHabbo().HasFuse("fuse_mod"))
            {
                Session.GetHabbo().SpamProtectionTime = (Room.IsRoleplay || Session.GetHabbo().HasFuse("fuse_low_flood")) ? 5 : 30;
                Session.GetHabbo().SpamEnable = true;

                User.GetClient().SendPacket(new FloodControlComposer(Session.GetHabbo().SpamProtectionTime - timeSpan.Seconds));

                return;
            }
            else if (Message.Length > 40 && Message == User.LastMessage && User.LastMessageCount == 1)
            {
                User.LastMessageCount = 0;
                User.LastMessage = "";

                Session.GetHabbo().SpamProtectionTime = (Room.IsRoleplay || Session.GetHabbo().HasFuse("fuse_low_flood")) ? 5 : 30;
                Session.GetHabbo().SpamEnable = true;
                User.GetClient().SendPacket(new FloodControlComposer(Session.GetHabbo().SpamProtectionTime - timeSpan.Seconds));
                return;
            }
            else
            {
                if (Message == User.LastMessage && Message.Length > 40)
                {
                    User.LastMessageCount++;
                }

                User.LastMessage = Message;

                Session.GetHabbo().SpamFloodTime = DateTime.Now;
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

                    if (Session.GetHabbo().IgnoreAll)
                    {
                        return;
                    }

                    foreach (string Username in User.WhiperGroupUsers.ToArray())
                    {
                        RoomUser UserWhiper = Room.GetRoomUserManager().GetRoomUserByName(Username);

                        if (UserWhiper == null || UserWhiper.GetClient() == null || UserWhiper.GetClient().GetHabbo() == null)
                        {
                            User.WhiperGroupUsers.Remove(Username);
                            continue;
                        }

                        if (UserWhiper.IsSpectator || UserWhiper.IsBot || UserWhiper.UserId == User.UserId || UserWhiper.GetClient().GetHabbo().MutedUsers.Contains(Session.GetHabbo().Id))
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

                    WhisperComposer MessageWhipser = new WhisperComposer(User.VirtualId, ButterflyEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", Session.Langue) + ToUser + ": " + Message, Color);
               
                    foreach (RoomUser roomUser in roomUserByRank)
                    {
                        if (roomUser != null && roomUser.HabboId != User.HabboId && roomUser.GetClient() != null && roomUser.GetClient().GetHabbo().ViewMurmur && !User.WhiperGroupUsers.Contains(roomUser.GetUsername()))
                        {
                            roomUser.GetClient().SendPacket(MessageWhipser);
                        }
                    }
                }
                else
                {
                    User.GetClient().SendPacket(new WhisperComposer(User.VirtualId, Message, Color));

                    if (Session.GetHabbo().IgnoreAll)
                    {
                        return;
                    }

                    RoomUser UserWhiper = Room.GetRoomUserManager().GetRoomUserByName(ToUser);

                    if (UserWhiper == null || UserWhiper.GetClient() == null || UserWhiper.GetClient().GetHabbo() == null)
                    {
                        return;
                    }

                    if (UserWhiper.IsSpectator || UserWhiper.IsBot || UserWhiper.UserId == User.UserId || UserWhiper.GetClient().GetHabbo().MutedUsers.Contains(Session.GetHabbo().Id))
                    {
                        return;
                    }

                    UserWhiper.GetClient().SendPacket(new WhisperComposer(User.VirtualId, Message, Color));

                    List<RoomUser> roomUserByRank = Room.GetRoomUserManager().GetStaffRoomUser();
                    if (roomUserByRank.Count <= 0)
                    {
                        return;
                    }

                    WhisperComposer MessageWhipserStaff = new WhisperComposer(User.VirtualId, ButterflyEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", Session.Langue) + ToUser + ": " + Message, Color);
                    foreach (RoomUser roomUser in roomUserByRank)
                    {
                        if (roomUser != null && roomUser.GetClient() != null && roomUser.GetClient().GetHabbo() != null && roomUser.HabboId != User.HabboId && roomUser.GetClient() != null && roomUser.GetClient().GetHabbo().ViewMurmur && UserWhiper.UserId != roomUser.UserId)
                        {
                            roomUser.GetClient().SendPacket(MessageWhipserStaff);
                        }
                    }
                }

                Session.GetHabbo().GetChatMessageManager().AddMessage(User.UserId, User.GetUsername(), User.RoomId, ButterflyEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", Session.Langue) + ToUser + ": " + Message, UnixTimestamp.GetNow());
                Room.GetChatMessageManager().AddMessage(User.UserId, User.GetUsername(), User.RoomId, ButterflyEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", Session.Langue) + ToUser + ": " + Message, UnixTimestamp.GetNow());
            }
        }
    }
}
