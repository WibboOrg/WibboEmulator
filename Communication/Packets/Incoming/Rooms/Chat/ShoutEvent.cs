using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Roleplay.Player;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Chat.Styles;
using WibboEmulator.Utilities;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class ShoutEvent : IPacketEvent
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

            string message = StringCharFilter.Escape(packet.PopString());
            if (message.Length > 100)
            {
                message = message[..100];
            }

            int Colour = packet.PopInt();

            if (!WibboEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(Colour, out ChatStyle Style) || (Style.RequiredRight.Length > 0 && !session.GetUser().HasPermission(Style.RequiredRight)))
            {
                Colour = 0;
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
                    user.GetClient().SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("user.muted", session.Langue));
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

                if (session != null)
                {
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
                }

                if (message.StartsWith(":", StringComparison.CurrentCulture) && WibboEnvironment.GetGame().GetChatManager().GetCommands().Parse(session, user, room, message))
                {
                    room.GetChatMessageManager().AddMessage(session.GetUser().Id, session.GetUser().Username, room.Id, string.Format("{0} a utilisé la commande {1}", session.GetUser().Username, message), UnixTimestamp.GetNow());
                    return;
                }

                if (session != null && !user.IsBot)
                {
                    if (session.Antipub(message, "<TCHAT>"))
                    {
                        return;
                    }

                    WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.SOCIAL_CHAT, 0);
                    session.GetUser().GetChatMessageManager().AddMessage(session.GetUser().Id, session.GetUser().Username, room.Id, message, UnixTimestamp.GetNow());
                    room.GetChatMessageManager().AddMessage(session.GetUser().Id, session.GetUser().Username, room.Id, message, UnixTimestamp.GetNow());

                    if (user.TransfBot)
                    {
                        Colour = 2;
                    }
                }
            }

            if (!session.GetUser().HasPermission("perm_word_filter_override"))
            {
                message = WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(message);
            }

            if (room.AllowsShous(user, message))
            {
                user.SendWhisperChat(message, true);
                return;
            }

            room.OnUserSay(user, message, true);

            if (user.IsSpectator)
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

            user.OnChat(message, Colour, true);

        }
    }
}
