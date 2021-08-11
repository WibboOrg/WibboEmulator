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

using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Quests;
using Butterfly.HabboHotel.Roleplay.Player;
using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.Rooms.Chat.Styles;
using Butterfly.Utilities;
using System;
using System.Text.RegularExpressions;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ChatEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
            {
                return;
            }

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            if (Room.IsRoleplay)
            {
                RolePlayer Rp = User.Roleplayer;
                if (Rp != null && Rp.Dead)
                {
                    return;
                }
            }

            string Message = Packet.PopString();

            if (Message.Length > 100)
            {
                Message = Message.Substring(0, 100);
            }

            int Colour = Packet.PopInt();

            if (!ButterflyEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(Colour, out ChatStyle Style) || (Style.RequiredRight.Length > 0 && !Session.GetHabbo().HasFuse(Style.RequiredRight)))
            {
                Colour = 0;
            }

            if (Colour != 23)
            {
                Message = StringCharFilter.Escape(Message);
            }

            User.Unidle();

            if (Session.GetHabbo().Rank < 5U && Room.RoomMuted && !User.IsOwner() && !Session.GetHabbo().CurrentRoom.CheckRights(Session))
            {
                User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("room.muted", Session.Langue));
                return;
            }

            if (Room.GetJanken().PlayerStarted(User))
            {
                if (!Room.GetJanken().PickChoix(User, Message))
                {
                    User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.choice", Session.Langue));
                }

                return;
            }

            if (Room.UserIsMuted(Session.GetHabbo().Id))
            {
                if (!Room.HasMuteExpired(Session.GetHabbo().Id))
                {
                    User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("user.muted", Session.Langue));
                    return;
                }
                else
                {
                    Room.RemoveMute(Session.GetHabbo().Id);
                }
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

                User.LastMessage = Message;

                Session.GetHabbo().spamFloodTime = DateTime.Now;
                User.FloodCount++;

                if (Message.StartsWith("@red@") || Message.StartsWith("@rouge@"))
                {
                    User.ChatTextColor = "@red@";
                }
                else if (Message.StartsWith("@cyan@"))
                {
                    User.ChatTextColor = "@cyan@";
                }
                else if (Message.StartsWith("@blue@") || Message.StartsWith("@bleu@"))
                {
                    User.ChatTextColor = "@blue@";
                }
                else if (Message.StartsWith("@green@") || Message.StartsWith("@vert@"))
                {
                    User.ChatTextColor = "@green@";
                }
                else if (Message.StartsWith("@purple@") || Message.StartsWith("@violet@"))
                {
                    User.ChatTextColor = "@purple@";
                }
                else if (Message.StartsWith("@black@") || Message.StartsWith("@noir@"))
                {
                    User.ChatTextColor = "";
                }

                if (Message.StartsWith(":", StringComparison.CurrentCulture) && ButterflyEnvironment.GetGame().GetChatManager().GetCommands().Parse(Session, User, Room, Message))
                {
                    Room.GetChatMessageManager().AddMessage(Session.GetHabbo().Id, Session.GetHabbo().Username, Room.Id, string.Format("{0} a utiliser la commande {1}", Session.GetHabbo().Username, Message));
                    return;
                }

                if (Session.Antipub(Message, "<TCHAT>", Room.Id))
                {
                    return;
                }

                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_CHAT, 0);
                Session.GetHabbo().GetChatMessageManager().AddMessage(Session.GetHabbo().Id, Session.GetHabbo().Username, Room.Id, Message);
                Room.GetChatMessageManager().AddMessage(Session.GetHabbo().Id, Session.GetHabbo().Username, Room.Id, Message);

                if (User.transfbot)
                {
                    Colour = 2;
                }
            }

            if (!Session.GetHabbo().HasFuse("word_filter_override"))
            {
                Message = ButterflyEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Message);
                Message = new Regex(@"\[tag\](.*?)\[\/tag\]").Replace(Message, "<tag>$1</tag>");
            }

            if (Room.AllowsShous(User, Message))
            {
                User.SendWhisperChat(Message, false);
                return;
            }

            Room.OnUserSay(User, Message, false);

            if (User.IsSpectator)
            {
                return;
            }

            if (User.muted && !Session.GetHabbo().HasFuse("fuse_tool"))
            {
                User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("user.muted", Session.Langue));
                return;
            }

            if (!Session.GetHabbo().IgnoreAll)
            {
                Message = ButterflyEnvironment.GetGame().GetChatManager().GetMention().Parse(Session, Message);
            }

            if (!string.IsNullOrEmpty(User.ChatTextColor))
            {
                Message = User.ChatTextColor + " " + Message;
            }

            User.OnChat(Message, Colour, false);
        }
    }
}
