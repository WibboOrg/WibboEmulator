using Butterfly.Communication.Packets.Outgoing.Rooms.Notifications;
using Butterfly.Communication.Packets.Outgoing.WebSocket;

using Butterfly.Game.GameClients;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Butterfly.Game.Rooms.Chat.Mentions
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
            GameClient TargetClient = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(TargetUsername);

            if (TargetClient == null)
            {
                return false;
            }

            if (TargetClient == Session)
            {
                return false;
            }

            if (TargetClient.GetHabbo() == null || TargetClient.GetHabbo().GetMessenger() == null)
            {
                return false;
            }

            if (!TargetClient.GetHabbo().GetMessenger().FriendshipExists(Session.GetHabbo().Id) && !Session.GetHabbo().HasFuse("fuse_mention"))
            {
                Session.SendPacket(RoomNotificationComposer.SendBubble("error", $"Tu as besoin d'être ami avec {TargetUsername} pour pouvoir le taguer"));
                return false;
            }

            if (!TargetClient.GetHabbo().SendWebPacket(new MentionComposer(Session.GetHabbo().Id, Session.GetHabbo().Username, Session.GetHabbo().Look, Message)))
            {
                TargetClient.SendPacket(RoomNotificationComposer.SendBubble("mention", $"{Session.GetHabbo().Username} t'a tagué:\n\n{Message}", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
            }

            return true;
        }

        /*public bool EveryoneFriend(GameClient Session, string Message)
        {
            TimeSpan timeSpan = DateTime.Now - Session.GetHabbo().everyoneTimer;
            if (timeSpan.TotalSeconds < 120)
            {
                Session.SendPacket(RoomNotificationComposer.SendBubble("error", $"Veuille attendre 2 minute avant de pouvoir réutiliser @everyone"));
                return false;
            }

            Session.GetHabbo().everyoneTimer = DateTime.Now;

            List<GameClient> onlineUsers = ButterflyEnvironment.GetGame().GetClientManager().GetClientsById(Session.GetHabbo().GetMessenger().GetFriends().Keys);

            if (onlineUsers == null)
            {
                return false;
            }

            foreach (GameClient TargetClient in onlineUsers)
            {
                if (TargetClient != null && TargetClient.GetHabbo() != null && TargetClient.GetHabbo().GetMessenger() != null)
                {
                    if (TargetClient.GetHabbo().GetMessenger().FriendshipExists(Session.GetHabbo().Id))
                    {
                        if (!TargetClient.GetHabbo().SendWebPacket(new MentionComposer(Session.GetHabbo().Id, Session.GetHabbo().Username, Session.GetHabbo().Look, Message)))
                        {
                            TargetClient.SendPacket(RoomNotificationComposer.SendBubble("mention", $"{Session.GetHabbo().Username} t'a tagué:\n\n{Message}", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
                        }
                    }
                }
            }
        }*/


        public bool EveryoneFriend(GameClient Session, string Message)
        {
            if (Session.GetHabbo().Rank < 2)
            {
                Session.SendPacket(RoomNotificationComposer.SendBubble("error", $"Vous devez être Premium pour utiliser @everyone"));
                return false;
            }

            TimeSpan timeSpan = DateTime.Now - Session.GetHabbo().everyoneTimer;
            if (timeSpan.TotalSeconds < 120)
            {
                Session.SendPacket(RoomNotificationComposer.SendBubble("error", $"Veuille attendre 2 minute avant de pouvoir réutiliser @everyone"));
                return false;
            }

            Session.GetHabbo().everyoneTimer = DateTime.Now;

            List<GameClient> onlineUsers = ButterflyEnvironment.GetGame().GetClientManager().GetClientsById(Session.GetHabbo().GetMessenger().GetFriends().Keys);

            if (onlineUsers == null)
            {
                return false;
            }

            foreach (GameClient TargetClient in onlineUsers)
            {
                if (TargetClient != null && TargetClient.GetHabbo() != null && TargetClient.GetHabbo().GetMessenger() != null)
                {
                    if (TargetClient.GetHabbo().GetMessenger().FriendshipExists(Session.GetHabbo().Id))
                    {
                        if (!TargetClient.GetHabbo().SendWebPacket(new MentionComposer(Session.GetHabbo().Id, Session.GetHabbo().Username, Session.GetHabbo().Look, Message)))
                        {
                            TargetClient.SendPacket(RoomNotificationComposer.SendBubble("mention", $"{Session.GetHabbo().Username} t'a tagué:\n\n{Message}", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
                        }
                    }
                }
            }

            return true;
        }
    }
}
