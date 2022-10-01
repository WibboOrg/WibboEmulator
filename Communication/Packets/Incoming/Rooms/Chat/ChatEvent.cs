using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;

using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Roleplay.Player;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Chat.Styles;
using WibboEmulator.Utilities;
using System.Text.RegularExpressions;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class ChatEvent : IPacketEvent
    {
        public double Delay => 100;

        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetUser() == null || !session.GetUser().InRoom)
            {
                return;
            }

            Room room = session.GetUser().CurrentRoom;
            if (room == null)
            {
                return;
            }

            RoomUser user = room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
            if (user == null)
            {
                return;
            }

            if (room.IsRoleplay)
            {
                RolePlayer Rp = user.Roleplayer;
                if (Rp != null && Rp.Dead)
                {
                    return;
                }
            }

            string message = packet.PopString();

            if (message.Length > 100)
            {
                message = message.Substring(0, 100);
            }

            int color = packet.PopInt();

            if (!WibboEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(color, out ChatStyle Style) || (Style.RequiredRight.Length > 0 && !session.GetUser().HasPermission(Style.RequiredRight)))
            {
                color = 0;
            }

            if (color != 23)
            {
                message = StringCharFilter.Escape(message);
            }

            user.Unidle();

            if (!session.GetUser().HasPermission("perm_mod") && !user.IsOwner() && !session.GetUser().CurrentRoom.CheckRights(session) && room.RoomMuted)
            {
                user.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("room.muted", session.Langue));
                return;
            }

            if (room.GetJanken().PlayerStarted(user))
            {
                if (!room.GetJanken().PickChoice(user, message))
                {
                    user.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("janken.choice", session.Langue));
                }

                return;
            }

            if (!session.GetUser().HasPermission("perm_mod") && !user.IsOwner() && !session.GetUser().CurrentRoom.CheckRights(session) && room.UserIsMuted(session.GetUser().Id))
            {
                if (!room.HasMuteExpired(session.GetUser().Id))
                {
                    user.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("user.muted", session.Langue));
                    return;
                }
                else
                {
                    room.RemoveMute(session.GetUser().Id);
                }
            }

            TimeSpan timeSpan = DateTime.Now - session.GetUser().SpamFloodTime;
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
                int i = session.GetUser().SpamProtectionTime - timeSpan.Seconds;
                user.GetClient().SendPacket(new FloodControlComposer(i));
                return;
            }
            else if (timeSpan.TotalSeconds < 4.0 && session.GetUser().FloodCount > 5 && !session.GetUser().HasPermission("perm_mod"))
            {
                session.GetUser().SpamProtectionTime = (room.IsRoleplay || session.GetUser().HasPermission("perm_flood_premium")) ? 5 : 15;
                session.GetUser().SpamEnable = true;

                user.GetClient().SendPacket(new FloodControlComposer(session.GetUser().SpamProtectionTime - timeSpan.Seconds));

                return;
            }
            else if (message.Length > 40 && message == user.LastMessage && user.LastMessageCount == 1)
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
                if (message == user.LastMessage && message.Length > 40)
                {
                    user.LastMessageCount++;
                }

                user.LastMessage = message;

                session.GetUser().SpamFloodTime = DateTime.Now;
                session.GetUser().FloodCount++;

                if (message.StartsWith("@red@") || message.StartsWith("@rouge@"))
                {
                    user.ChatTextColor = "@red@";
                }
                else if (message.StartsWith("@cyan@"))
                {
                    user.ChatTextColor = "@cyan@";
                }
                else if (message.StartsWith("@blue@") || message.StartsWith("@bleu@"))
                {
                    user.ChatTextColor = "@blue@";
                }
                else if (message.StartsWith("@green@") || message.StartsWith("@vert@"))
                {
                    user.ChatTextColor = "@green@";
                }
                else if (message.StartsWith("@purple@") || message.StartsWith("@violet@"))
                {
                    user.ChatTextColor = "@purple@";
                }
                else if (message.StartsWith("@black@") || message.StartsWith("@noir@"))
                {
                    user.ChatTextColor = "";
                }

                if (message.StartsWith(":", StringComparison.CurrentCulture) && WibboEnvironment.GetGame().GetChatManager().GetCommands().Parse(session, user, room, message))
                {
                    room.GetChatMessageManager().AddMessage(session.GetUser().Id, session.GetUser().Username, room.Id, string.Format("{0} a utilisé la commande {1}", session.GetUser().Username, message), UnixTimestamp.GetNow());
                    return;
                }

                if (session.Antipub(message, "<TCHAT>", room.Id))
                {
                    return;
                }

                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.SOCIAL_CHAT, 0);
                session.GetUser().GetChatMessageManager().AddMessage(session.GetUser().Id, session.GetUser().Username, room.Id, message, UnixTimestamp.GetNow());
                room.GetChatMessageManager().AddMessage(session.GetUser().Id, session.GetUser().Username, room.Id, message, UnixTimestamp.GetNow());

                if (user.TransfBot)
                {
                    color = 2;
                }
            }

            if (!session.GetUser().HasPermission("perm_word_filter_override"))
            {
                message = WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(message);
                message = new Regex(@"\[tag\](.*?)\[\/tag\]").Replace(message, "<tag>$1</tag>");
            }

            if (room.AllowsShous(user, message))
            {
                user.SendWhisperChat(message, false);
                return;
            }

            room.OnUserSay(user, message, false);

            if (user.IsSpectator && session.GetUser().Rank < 11)
            {
                return;
            }

            if (!session.GetUser().IgnoreAll)
            {
                message = WibboEnvironment.GetGame().GetChatManager().GetMention().Parse(session, message);
            }

            if (!string.IsNullOrEmpty(user.ChatTextColor))
            {
                message = user.ChatTextColor + " " + message;
            }

            user.OnChat(message, color, false);
        }
    }
}
