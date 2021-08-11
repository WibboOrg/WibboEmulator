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

using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.Rooms.RoomBots;
using System;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class SaveBotActionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            int BotId = Packet.PopInt();
            int ActionId = Packet.PopInt();
            string DataString = Packet.PopString();

            if (BotId <= 0)
            {
                return;
            }

            if (ActionId < 1 || ActionId > 5)
            {
                return;
            }

            if (!Room.GetRoomUserManager().TryGetBot(BotId, out RoomUser Bot))
            {
                return;
            }

            RoomBot RoomBot = Bot.BotData;
            if (RoomBot == null)
            {
                return;
            }

            /* 1 = Copy looks
             * 2 = Setup Speech
             * 3 = Relax
             * 4 = Dance
             * 5 = Change Name
             */

            switch (ActionId)
            {
                #region Copy Looks (1)
                case 1:
                    {
                        //Change the defaults
                        Bot.BotData.Look = Session.GetHabbo().Look;
                        Bot.BotData.Gender = Session.GetHabbo().Gender;

                        Room.SendPacket(new UserChangeComposer(Bot));

                        using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `bots` SET `look` = @look, `gender` = '" + Session.GetHabbo().Gender + "' WHERE `id` = '" + Bot.BotData.Id + "' LIMIT 1");
                            dbClient.AddParameter("look", Session.GetHabbo().Look);
                            dbClient.RunQuery();
                        }
                        break;
                    }
                #endregion

                #region Setup Speech (2)
                case 2:
                    {

                        string[] ConfigData = DataString.Split(new string[]
                        {
                            ";#;"
                        }, StringSplitOptions.None);

                        string[] SpeechData = ConfigData[0].Split(new char[]
                        {
                            '\r',
                            '\n'
                        }, StringSplitOptions.RemoveEmptyEntries);

                        string AutomaticChat = Convert.ToString(ConfigData[1]);
                        string SpeakingInterval = Convert.ToString(ConfigData[2]);
                        string MixChat = Convert.ToString(ConfigData[3]);

                        if (string.IsNullOrEmpty(SpeakingInterval) || Convert.ToInt32(SpeakingInterval) <= 0 || Convert.ToInt32(SpeakingInterval) < 7)
                        {
                            SpeakingInterval = "7";
                        }

                        RoomBot.AutomaticChat = Convert.ToBoolean(AutomaticChat);
                        RoomBot.SpeakingInterval = Convert.ToInt32(SpeakingInterval);
                        RoomBot.MixSentences = Convert.ToBoolean(MixChat);

                        string Text = "";
                        string Phrase = "";
                        for (int i = 0; i <= SpeechData.Length - 1; i++)
                        {
                            Phrase = SpeechData[i];
                            if (Phrase.Length > 150)
                            {
                                Phrase.Substring(0, 150);
                            }

                            Text += SpeechData[i] + "\r";
                        }

                        RoomBot.ChatText = Text;
                        RoomBot.LoadRandomSpeech(Text);

                        using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `bots` SET `chat_enabled` = @AutomaticChat, `chat_seconds` = @SpeakingInterval, `is_mixchat` = @MixChat, `chat_text` = @ChatText WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("id", BotId);
                            dbClient.AddParameter("AutomaticChat", ButterflyEnvironment.BoolToEnum(Convert.ToBoolean(AutomaticChat)));
                            dbClient.AddParameter("SpeakingInterval", Convert.ToInt32(SpeakingInterval));
                            dbClient.AddParameter("MixChat", ButterflyEnvironment.BoolToEnum(Convert.ToBoolean(MixChat)));
                            dbClient.AddParameter("ChatText", Text);
                            dbClient.RunQuery();
                        }

                        break;
                    }
                #endregion

                #region Relax (3)
                case 3:
                    {
                        Bot.BotData.WalkingEnabled = !Bot.BotData.WalkingEnabled;
                        using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            queryreactor.RunQuery("UPDATE bots SET walk_enabled = '" + ButterflyEnvironment.BoolToEnum(Bot.BotData.WalkingEnabled) + "' WHERE id = " + Bot.BotData.Id);
                        }
                        break;
                    }
                #endregion

                #region Dance (4)
                case 4:
                    {
                        if (Bot.DanceId > 0)
                        {
                            Bot.DanceId = 0;
                            Bot.BotData.IsDancing = false;
                        }
                        else
                        {
                            Random RandomDance = new Random();
                            Bot.DanceId = RandomDance.Next(1, 4);
                            Bot.BotData.IsDancing = true;
                        }

                        Room.SendPacket(new DanceComposer(Bot, Bot.DanceId));

                        using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            queryreactor.RunQuery("UPDATE bots SET is_dancing = '" + ButterflyEnvironment.BoolToEnum(Bot.BotData.IsDancing) + "' WHERE id = " + Bot.BotData.Id);
                        }

                        break;
                    }
                #endregion

                #region Change Name (5)
                case 5:
                    {
                        if (DataString.Length == 0)
                        {
                            return;
                        }
                        else if (DataString.Length >= 16)
                        {
                            return;
                        }

                        if (DataString.Contains("<img src") || DataString.Contains("<font ") || DataString.Contains("</font>") || DataString.Contains("</a>") || DataString.Contains("<i>"))
                        {
                            return;
                        }

                        Bot.BotData.Name = DataString;
                        using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `bots` SET `name` = @name WHERE `id` = '" + Bot.BotData.Id + "' LIMIT 1");
                            dbClient.AddParameter("name", DataString);
                            dbClient.RunQuery();
                        }
                        Room.SendPacket(new UserNameChangeMessageComposer(Bot.BotData.Name, Bot.VirtualId));
                        break;
                    }
                    #endregion
            }
        }
    }
}