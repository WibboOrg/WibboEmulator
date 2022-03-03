using Butterfly.Communication.Packets.Outgoing.Rooms.Notifications;
using Butterfly.Communication.Packets.Outgoing.WebSocket;

using Butterfly.Game.Clients;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Butterfly.Game.Chat.Mentions
{
    public class MentionManager
    {
        public string MentionPattern = @"@(.*?)(?=\s|$)";

        public string StylePrefix = "[tag]";
        public string StyleSuffix = "[/tag]";

        public string Parse(Client Session, string Message)
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

        public bool SendNotif(Client Session, string TargetUsername, string Message)
        {
            Client TargetClient = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(TargetUsername);

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

            if (!TargetClient.GetUser().GetMessenger().FriendshipExists(Session.GetUser().Id) && !Session.GetUser().HasFuse("fuse_mention"))
            {
                Session.SendPacket(RoomNotificationComposer.SendBubble("error", $"Tu as besoin d'être ami avec {TargetUsername} pour pouvoir le taguer"));
                return false;
            }

            if (!TargetClient.GetUser().SendWebPacket(new MentionComposer(Session.GetUser().Id, Session.GetUser().Username, Session.GetUser().Look, Message)))
            {
                TargetClient.SendPacket(RoomNotificationComposer.SendBubble("mention", $"{Session.GetUser().Username} t'a tagué:\n\n{Message}", "event:navigator/goto/" + Session.GetUser().CurrentRoomId));
            }

            return true;
        }

        public bool EveryoneFriend(Client Session, string Message)
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

            List<Client> onlineUsers = ButterflyEnvironment.GetGame().GetClientManager().GetClientsById(Session.GetUser().GetMessenger().Friends.Keys);

            if (onlineUsers == null)
            {
                return false;
            }

            foreach (Client TargetClient in onlineUsers)
            {
                if (TargetClient != null && TargetClient.GetUser() != null && TargetClient.GetUser().GetMessenger() != null)
                {
                    if (TargetClient.GetUser().GetMessenger().FriendshipExists(Session.GetUser().Id))
                    {
                        if (!TargetClient.GetUser().SendWebPacket(new MentionComposer(Session.GetUser().Id, Session.GetUser().Username, Session.GetUser().Look, Message)))
                        {
                            TargetClient.SendPacket(RoomNotificationComposer.SendBubble("mention", $"{Session.GetUser().Username} t'a tagué:\n\n{Message}", "event:navigator/goto/" + Session.GetUser().CurrentRoomId));
                        }
                    }
                }
            }

            return true;
        }
    }
}
