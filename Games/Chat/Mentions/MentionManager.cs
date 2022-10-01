using WibboEmulator.Communication.Packets.Outgoing.Notifications.NotifCustom;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;


using WibboEmulator.Games.GameClients;
using System.Text.RegularExpressions;

namespace WibboEmulator.Games.Chat.Mentions
{
    public class MentionManager
    {
        public string MentionPattern = @"@(.*?)(?=\s|$)";

        public string StylePrefix = "[tag]";
        public string StyleSuffix = "[/tag]";

        public string Parse(GameClient Session, string Message)
        {
            string StyledMessage = Message;

            int ChangeLength = this.StylePrefix.Length + this.StyleSuffix.Length;
            int ChangeCount = 0;
            List<string> UsersTarget = new List<string>();

            foreach (Match m in Regex.Matches(Message, this.MentionPattern))
            {
                string TargetUsername = m.Groups[1].Value.ToLower();

                if (string.IsNullOrWhiteSpace(TargetUsername))
                {
                    continue;
                }

                if (UsersTarget.Contains(TargetUsername))
                {
                    continue;
                }

                UsersTarget.Add(TargetUsername);

                if (TargetUsername == "everyone")
                {
                    if (!this.EveryoneFriend(Session, Message))
                    {
                        break;
                    }
                }

                else if (!this.SendNotif(Session, TargetUsername, Message))
                {
                    continue;
                }

                StyledMessage = StyledMessage.Insert(ChangeCount * ChangeLength + m.Index, this.StylePrefix);
                StyledMessage = StyledMessage.Insert(ChangeCount * ChangeLength + m.Index + this.StylePrefix.Length + m.Length, this.StyleSuffix);

                ChangeCount++;
            }

            return StyledMessage;
        }

        public bool SendNotif(GameClient Session, string TargetUsername, string Message)
        {
            GameClient TargetClient = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(TargetUsername);

            if (TargetClient == null)
            {
                return false;
            }

            if (TargetClient == Session)
            {
                return false;
            }

            if (TargetClient.GetUser() == null || TargetClient.GetUser().GetMessenger() == null)
            {
                return false;
            }

            if (!TargetClient.GetUser().GetMessenger().FriendshipExists(Session.GetUser().Id) && !Session.GetUser().HasPermission("perm_mention"))
            {
                Session.SendPacket(RoomNotificationComposer.SendBubble("error", $"Tu as besoin d'être ami avec {TargetUsername} pour pouvoir le taguer"));
                return false;
            }

            TargetClient.SendPacket(new MentionComposer(Session.GetUser().Id, Session.GetUser().Username, Session.GetUser().Look, Message));

            return true;
        }

        public bool EveryoneFriend(GameClient Session, string Message)
        {
            if (Session.GetUser().Rank < 2)
            {
                Session.SendPacket(RoomNotificationComposer.SendBubble("error", $"Vous devez être Premium pour utiliser @everyone"));
                return false;
            }

            TimeSpan timeSpan = DateTime.Now - Session.GetUser().EveryoneTimer;
            if (timeSpan.TotalSeconds < 120)
            {
                Session.SendPacket(RoomNotificationComposer.SendBubble("error", $"Veuille attendre 2 minute avant de pouvoir réutiliser @everyone"));
                return false;
            }

            Session.GetUser().EveryoneTimer = DateTime.Now;

            List<GameClient> onlineUsers = WibboEnvironment.GetGame().GetClientManager().GetClientsById(Session.GetUser().GetMessenger().Friends.Keys);

            if (onlineUsers == null)
            {
                return false;
            }

            foreach (GameClient TargetClient in onlineUsers)
            {
                if (TargetClient != null && TargetClient.GetUser() != null && TargetClient.GetUser().GetMessenger() != null)
                {
                    if (TargetClient.GetUser().GetMessenger().FriendshipExists(Session.GetUser().Id))
                    {
                        TargetClient.SendPacket(new MentionComposer(Session.GetUser().Id, Session.GetUser().Username, Session.GetUser().Look, Message));
                    }
                }
            }

            return true;
        }
    }
}
