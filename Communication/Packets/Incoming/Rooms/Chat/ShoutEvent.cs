using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Quests;
using WibboEmulator.Game.Roleplay.Player;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Chat.Styles;
using WibboEmulator.Utilities;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class ShoutEvent : IPacketEvent
    {
        public double Delay => 100;

        public void Parse(Client session, ClientPacket packet)
        {
            if (session == null || session.GetUser() == null || !session.GetUser().InRoom)
            {
                return;
            }

            Room Room = session.GetUser().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            RoomUser user = Room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
            if (user == null)
            {
                return;
            }

            if (Room.IsRoleplay)
            {
                RolePlayer Rp = user.Roleplayer;
                if (Rp != null && Rp.Dead)
                {
                    return;
                }
            }

            string Message = StringCharFilter.Escape(packet.PopString());
            if (Message.Length > 100)
            {
                Message = Message[..100];
            }

            int Colour = packet.PopInt();

            if (!WibboEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(Colour, out ChatStyle Style) || (Style.RequiredRight.Length > 0 && !session.GetUser().HasPermission(Style.RequiredRight)))
            {
                Colour = 0;
            }

            user.Unidle();

            if (!session.GetUser().HasPermission("perm_mod") && !user.IsOwner() && !session.GetUser().CurrentRoom.CheckRights(session) && Room.RoomMuted)
            {
                user.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("room.muted", session.Langue));
                return;
            }

            if (Room.GetJanken().PlayerStarted(user))
            {
                if (!Room.GetJanken().PickChoice(user, Message))
                {
                    user.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("janken.choice", session.Langue));
                }

                return;
            }
            if (!session.GetUser().HasPermission("perm_mod") && !user.IsOwner() && !session.GetUser().CurrentRoom.CheckRights(session) && Room.UserIsMuted(session.GetUser().Id))
            {
                if (!Room.HasMuteExpired(session.GetUser().Id))
                {
                    user.GetClient().SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("user.muted", session.Langue));
                    return;
                }
                else
                {
                    Room.RemoveMute(session.GetUser().Id);
                }
            }

            TimeSpan timeSpan = DateTime.Now - session.GetUser().SpamFloodTime;
            if (timeSpan.TotalSeconds > session.GetUser().SpamProtectionTime && session.GetUser().SpamEnable)
            {
                user.FloodCount = 0;
                session.GetUser().SpamEnable = false;
            }
            else if (timeSpan.TotalSeconds > 4.0)
            {
                user.FloodCount = 0;
            }

            if (timeSpan.TotalSeconds < session.GetUser().SpamProtectionTime && session.GetUser().SpamEnable)
            {
                int i = session.GetUser().SpamProtectionTime - timeSpan.Seconds;
                user.GetClient().SendPacket(new FloodControlComposer(i));
                return;
            }
            else if (timeSpan.TotalSeconds < 4.0 && user.FloodCount > 5 && !session.GetUser().HasPermission("perm_mod"))
            {
                session.GetUser().SpamProtectionTime = (Room.IsRoleplay || session.GetUser().HasPermission("perm_flood_premium")) ? 5 : 15;
                session.GetUser().SpamEnable = true;

                user.GetClient().SendPacket(new FloodControlComposer(session.GetUser().SpamProtectionTime - timeSpan.Seconds));

                return;
            }
            else if (Message.Length > 40 && Message == user.LastMessage && user.LastMessageCount == 1)
            {
                user.LastMessageCount = 0;
                user.LastMessage = "";

                session.GetUser().SpamProtectionTime = (Room.IsRoleplay || session.GetUser().HasPermission("perm_flood_premium")) ? 5 : 15;
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
                user.FloodCount++;

                if (session != null)
                {
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
                }

                if (Message.StartsWith(":", StringComparison.CurrentCulture) && WibboEnvironment.GetGame().GetChatManager().GetCommands().Parse(session, user, Room, Message))
                {
                    Room.GetChatMessageManager().AddMessage(session.GetUser().Id, session.GetUser().Username, Room.Id, string.Format("{0} a utilisé la commande {1}", session.GetUser().Username, Message), UnixTimestamp.GetNow());
                    return;
                }

                if (session != null && !user.IsBot)
                {
                    if (session.Antipub(Message, "<TCHAT>"))
                    {
                        return;
                    }

                    WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.SOCIAL_CHAT, 0);
                    session.GetUser().GetChatMessageManager().AddMessage(session.GetUser().Id, session.GetUser().Username, Room.Id, Message, UnixTimestamp.GetNow());
                    Room.GetChatMessageManager().AddMessage(session.GetUser().Id, session.GetUser().Username, Room.Id, Message, UnixTimestamp.GetNow());

                    if (user.TransfBot)
                    {
                        Colour = 2;
                    }
                }
            }

            if (!session.GetUser().HasPermission("perm_word_filter_override"))
            {
                Message = WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Message);
            }

            if (Room.AllowsShous(user, Message))
            {
                user.SendWhisperChat(Message, true);
                return;
            }

            Room.OnUserSay(user, Message, true);

            if (user.IsSpectator)
            {
                return;
            }

            if (!session.GetUser().IgnoreAll)
            {
                Message = WibboEnvironment.GetGame().GetChatManager().GetMention().Parse(session, Message);
            }

            if (!string.IsNullOrEmpty(user.ChatTextColor))
            {
                Message = user.ChatTextColor + " " + Message;
            }

            user.OnChat(Message, Colour, true);

        }
    }
}
