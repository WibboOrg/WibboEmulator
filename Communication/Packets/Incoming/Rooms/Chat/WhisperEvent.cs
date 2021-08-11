using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Avatar;
using Butterfly.Communication.Packets.Outgoing.BuildersClub;
using Butterfly.Communication.Packets.Outgoing.Camera;
using Butterfly.Communication.Packets.Outgoing.Catalog;
using Butterfly.Communication.Packets.Outgoing.GameCenter;
using Butterfly.Communication.Packets.Outgoing.Groups;
using Butterfly.Communication.Packets.Outgoing.Handshake;
using Butterfly.Communication.Packets.Outgoing.Help;
using Butterfly.Communication.Packets.Outgoing.Inventory;
using Butterfly.Communication.Packets.Outgoing.Inventory.Achievements;
using Butterfly.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Butterfly.Communication.Packets.Outgoing.Inventory.Badges;
using Butterfly.Communication.Packets.Outgoing.Inventory.Bots;
using Butterfly.Communication.Packets.Outgoing.Inventory.Furni;
using Butterfly.Communication.Packets.Outgoing.Inventory.Pets;
using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;
using Butterfly.Communication.Packets.Outgoing.Inventory.Trading;
using Butterfly.Communication.Packets.Outgoing.LandingView;
using Butterfly.Communication.Packets.Outgoing.MarketPlace;
using Butterfly.Communication.Packets.Outgoing.Messenger;
using Butterfly.Communication.Packets.Outgoing.Misc;
using Butterfly.Communication.Packets.Outgoing.Moderation;
using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Communication.Packets.Outgoing.Notifications;
using Butterfly.Communication.Packets.Outgoing.Pets;
using Butterfly.Communication.Packets.Outgoing.Quests;
using Butterfly.Communication.Packets.Outgoing.Rooms;
using Butterfly.Communication.Packets.Outgoing.Rooms.Action;
using Butterfly.Communication.Packets.Outgoing.Rooms.AI.Bots;
using Butterfly.Communication.Packets.Outgoing.Rooms.AI.Pets;
using Butterfly.Communication.Packets.Outgoing.Rooms.Avatar;
using Butterfly.Communication.Packets.Outgoing.Rooms.Chat;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Communication.Packets.Outgoing.Rooms.FloorPlan;
using Butterfly.Communication.Packets.Outgoing.Rooms.Freeze;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Furni;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Moodlight;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Stickys;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Wired;
using Butterfly.Communication.Packets.Outgoing.Rooms.Notifications;
using Butterfly.Communication.Packets.Outgoing.Rooms.Permissions;
using Butterfly.Communication.Packets.Outgoing.Rooms.Session;
using Butterfly.Communication.Packets.Outgoing.Rooms.Settings;
using Butterfly.Communication.Packets.Outgoing.Rooms.Wireds;
using Butterfly.Communication.Packets.Outgoing.Sound;
using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.HabboHotel.GameClients;using Butterfly.HabboHotel.Rooms;using Butterfly.HabboHotel.Rooms.Chat.Styles;using Butterfly.Utilities;using System;using System.Collections.Generic;namespace Butterfly.Communication.Packets.Incoming.Structure{    internal class WhisperEvent : IPacketEvent    {        public void Parse(GameClient Session, ClientPacket Packet)        {            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            Room Room = Session.GetHabbo().CurrentRoom;            if (Room == null)
            {
                return;
            }

            if (Room.UserIsMuted(Session.GetHabbo().Id))            {                if (!Room.HasMuteExpired(Session.GetHabbo().Id))
                {
                    return;
                }
                else
                {
                    Room.RemoveMute(Session.GetHabbo().Id);
                }
            }            string Params = StringCharFilter.Escape(Packet.PopString());            if (string.IsNullOrEmpty(Params) || Params.Length > 100 || !Params.Contains(" "))
            {
                return;
            }

            string ToUser = Params.Split(new char[1] { ' ' })[0];            if (ToUser.Length + 1 > Params.Length)
            {
                return;
            }

            string Message = Params.Substring(ToUser.Length + 1);            int Color = Packet.PopInt();

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

            TimeSpan timeSpan = DateTime.Now - Session.GetHabbo().spamFloodTime;
            if (timeSpan.TotalSeconds > Session.GetHabbo().spamProtectionTime && Session.GetHabbo().spamEnable)
            {
                User.FloodCount = 0;
                Session.GetHabbo().spamEnable = false;
            }
            else if (timeSpan.TotalSeconds > 4.0)
            {
                User.FloodCount = 0;
            }

            if (timeSpan.TotalSeconds < Session.GetHabbo().spamProtectionTime && Session.GetHabbo().spamEnable)
            {
                int i = Session.GetHabbo().spamProtectionTime - timeSpan.Seconds;
                User.GetClient().SendPacket(new FloodControlComposer(i));
                return;
            }
            else if (timeSpan.TotalSeconds < 4.0 && User.FloodCount > 5 && !Session.GetHabbo().HasFuse("fuse_mod"))
            {
                Session.GetHabbo().spamProtectionTime = (Room.IsRoleplay || Session.GetHabbo().HasFuse("fuse_low_flood")) ? 5 : 30;
                Session.GetHabbo().spamEnable = true;

                User.GetClient().SendPacket(new FloodControlComposer(Session.GetHabbo().spamProtectionTime - timeSpan.Seconds));

                return;
            }
            else if (Message.Length > 40 && Message == User.LastMessage && User.LastMessageCount == 1)
            {
                User.LastMessageCount = 0;
                User.LastMessage = "";

                Session.GetHabbo().spamProtectionTime = (Room.IsRoleplay || Session.GetHabbo().HasFuse("fuse_low_flood")) ? 5 : 30;
                Session.GetHabbo().spamEnable = true;
                User.GetClient().SendPacket(new FloodControlComposer(Session.GetHabbo().spamProtectionTime - timeSpan.Seconds));
                return;
            }
            else
            {
                if (Message == User.LastMessage && Message.Length > 40)
                {
                    User.LastMessageCount++;
                }

                User.LastMessage = Message;                Session.GetHabbo().spamFloodTime = DateTime.Now;                User.FloodCount++;                if (Message.StartsWith("@red@"))
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

                    ServerPacket Message1 = new ServerPacket(ServerPacketHeader.UNIT_CHAT_WHISPER);
                    Message1.WriteInteger(User.VirtualId);
                    Message1.WriteString(Message);
                    Message1.WriteInteger(ButterflyEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(Message));
                    Message1.WriteInteger(Color);
                    Message1.WriteInteger(0);
                    Message1.WriteInteger(-1);
                    User.GetClient().SendPacket(Message1);

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

                        UserWhiper.GetClient().SendPacket(Message1);
                    }

                    List<RoomUser> roomUserByRank = Room.GetRoomUserManager().GetStaffRoomUser();
                    if (roomUserByRank.Count <= 0)
                    {
                        return;
                    }

                    ServerPacket Message2 = new ServerPacket(ServerPacketHeader.UNIT_CHAT_WHISPER);
                    Message2.WriteInteger(User.VirtualId);
                    Message2.WriteString(ButterflyEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", Session.Langue) + ToUser + ": " + Message);
                    Message2.WriteInteger(ButterflyEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(Message));
                    Message2.WriteInteger(Color);
                    Message2.WriteInteger(0);
                    Message2.WriteInteger(-1);
                    foreach (RoomUser roomUser in roomUserByRank)
                    {
                        if (roomUser != null && roomUser.HabboId != User.HabboId && roomUser.GetClient() != null && roomUser.GetClient().GetHabbo().ViewMurmur && !User.WhiperGroupUsers.Contains(roomUser.GetUsername()))
                        {
                            roomUser.GetClient().SendPacket(Message2);
                        }
                    }
                }
                else
                {
                    ServerPacket Message1 = new ServerPacket(ServerPacketHeader.UNIT_CHAT_WHISPER);
                    Message1.WriteInteger(User.VirtualId);
                    Message1.WriteString(Message);
                    Message1.WriteInteger(ButterflyEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(Message));
                    Message1.WriteInteger(Color);
                    Message1.WriteInteger(0);
                    Message1.WriteInteger(-1);
                    User.GetClient().SendPacket(Message1);

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

                    UserWhiper.GetClient().SendPacket(Message1);

                    List<RoomUser> roomUserByRank = Room.GetRoomUserManager().GetStaffRoomUser();
                    if (roomUserByRank.Count <= 0)
                    {
                        return;
                    }

                    ServerPacket Message2 = new ServerPacket(ServerPacketHeader.UNIT_CHAT_WHISPER);
                    Message2.WriteInteger(User.VirtualId);
                    Message2.WriteString(ButterflyEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", Session.Langue) + ToUser + ": " + Message);
                    Message2.WriteInteger(ButterflyEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(Message));
                    Message2.WriteInteger(Color);
                    Message2.WriteInteger(0);
                    Message2.WriteInteger(-1);
                    foreach (RoomUser roomUser in roomUserByRank)
                    {
                        if (roomUser != null && roomUser.HabboId != User.HabboId && roomUser.GetClient() != null && roomUser.GetClient().GetHabbo().ViewMurmur && UserWhiper.UserId != roomUser.UserId)
                        {
                            roomUser.GetClient().SendPacket(Message2);
                        }
                    }
                }

                Session.GetHabbo().GetChatMessageManager().AddMessage(User.UserId, User.GetUsername(), User.RoomId, ButterflyEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", Session.Langue) + ToUser + ": " + Message);
                Room.GetChatMessageManager().AddMessage(User.UserId, User.GetUsername(), User.RoomId, ButterflyEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", Session.Langue) + ToUser + ": " + Message);            }        }    }}