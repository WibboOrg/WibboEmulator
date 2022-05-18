using Butterfly.Communication.Packets.Outgoing.Rooms.Chat;
using Butterfly.Game.Clients;
using Butterfly.Game.Quests;
using Butterfly.Game.Roleplay.Player;
using Butterfly.Game.Rooms;
using Butterfly.Game.Chat.Styles;
using Butterfly.Utilities;
using System;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ShoutEvent : IPacketEvent
    {
        public double Delay => 100;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null || !Session.GetUser().InRoom)
            {
                return;
            }

            Room Room = Session.GetUser().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
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

            string Message = StringCharFilter.Escape(Packet.PopString());
            if (Message.Length > 100)
            {
                Message = Message.Substring(0, 100);
            }

            int Colour = Packet.PopInt();

            if (!ButterflyEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(Colour, out ChatStyle Style) || (Style.RequiredRight.Length > 0 && !Session.GetUser().HasFuse(Style.RequiredRight)))
            {
                Colour = 0;
            }

            User.Unidle();

            if (Session.GetUser().Rank < 5U && Room.RoomMuted && !User.IsOwner() && !Session.GetUser().CurrentRoom.CheckRights(Session))
            {
                User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("room.muted", Session.Langue));
                return;
            }

            if (Room.GetJanken().PlayerStarted(User))
            {
                if (!Room.GetJanken().PickChoice(User, Message))
                {
                    User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.choice", Session.Langue));
                }

                return;
            }
            if (Room.UserIsMuted(Session.GetUser().Id))
            {
                if (!Room.HasMuteExpired(Session.GetUser().Id))
                {
                    User.GetClient().SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("user.muted", Session.Langue));
                    return;
                }
                else
                {
                    Room.RemoveMute(Session.GetUser().Id);
                }
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
                int i = Session.GetUser().SpamProtectionTime - timeSpan.Seconds;
                User.GetClient().SendPacket(new FloodControlComposer(i));
                return;
            }
            else if (timeSpan.TotalSeconds < 4.0 && User.FloodCount > 5 && !Session.GetUser().HasFuse("fuse_mod"))
            {
                Session.GetUser().SpamProtectionTime = (Room.IsRoleplay || Session.GetUser().HasFuse("fuse_low_flood")) ? 5 : 15;
                Session.GetUser().SpamEnable = true;

                User.GetClient().SendPacket(new FloodControlComposer(Session.GetUser().SpamProtectionTime - timeSpan.Seconds));

                return;
            }
            else if (Message.Length > 40 && Message == User.LastMessage && User.LastMessageCount == 1)
            {
                User.LastMessageCount = 0;
                User.LastMessage = "";

                Session.GetUser().SpamProtectionTime = (Room.IsRoleplay || Session.GetUser().HasFuse("fuse_low_flood")) ? 5 : 15;
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

                if (Session != null)
                {
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
                }

                if (Message.StartsWith(":", StringComparison.CurrentCulture) && ButterflyEnvironment.GetGame().GetChatManager().GetCommands().Parse(Session, User, Room, Message))
                {
                    Room.GetChatMessageManager().AddMessage(Session.GetUser().Id, Session.GetUser().Username, Room.Id, string.Format("{0} a utilisé la commande {1}", Session.GetUser().Username, Message), UnixTimestamp.GetNow());
                    return;
                }

                if (Session != null && !User.IsBot)
                {
                    if (Session.Antipub(Message, "<TCHAT>"))
                    {
                        return;
                    }

                    ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_CHAT, 0);
                    Session.GetUser().GetChatMessageManager().AddMessage(Session.GetUser().Id, Session.GetUser().Username, Room.Id, Message, UnixTimestamp.GetNow());
                    Room.GetChatMessageManager().AddMessage(Session.GetUser().Id, Session.GetUser().Username, Room.Id, Message, UnixTimestamp.GetNow());

                    if (User.TransfBot)
                    {
                        Colour = 2;
                    }
                }
            }

            if (!Session.GetUser().HasFuse("fuse_word_filter_override"))
            {
                Message = ButterflyEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Message);
            }

            if (Room.AllowsShous(User, Message))
            {
                User.SendWhisperChat(Message, true);
                return;
            }

            Room.OnUserSay(User, Message, true);

            if (User.IsSpectator)
            {
                return;
            }

            if (!Session.GetUser().IgnoreAll)
            {
                Message = ButterflyEnvironment.GetGame().GetChatManager().GetMention().Parse(Session, Message);
            }

            if (!string.IsNullOrEmpty(User.ChatTextColor))
            {
                Message = User.ChatTextColor + " " + Message;
            }

            User.OnChat(Message, Colour, true);

        }
    }
}
