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
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Butterfly.HabboHotel.Rooms.Chat.Mentions
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

            if (!TargetClient.GetHabbo().GetMessenger().FriendshipExists(Session.GetHabbo().Id))
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
