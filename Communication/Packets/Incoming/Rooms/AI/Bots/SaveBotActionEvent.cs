using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Rooms.AI;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class SaveBotActionEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetUser().InRoom)
            {
                return;
            }

            Room Room = Session.GetUser().CurrentRoom;
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
                        Bot.BotData.Look = Session.GetUser().Look;
                        Bot.BotData.Gender = Session.GetUser().Gender;

                        Room.SendPacket(new UserChangeComposer(Bot));

                        using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                        BotUserDao.UpdateLookGender(dbClient, Bot.BotData.Id, Session.GetUser().Gender, Session.GetUser().Look);
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
                        bool SpeakingIntervalIsInt = int.TryParse(ConfigData[2], out int SpeakingInterval);
                        string MixChat = Convert.ToString(ConfigData[3]);

                        if (SpeakingInterval <= 0 || SpeakingInterval < 7 || !SpeakingIntervalIsInt)
                        {
                            SpeakingInterval = 7;
                        }

                        RoomBot.AutomaticChat = Convert.ToBoolean(AutomaticChat);
                        RoomBot.SpeakingInterval = SpeakingInterval;
                        RoomBot.MixSentences = Convert.ToBoolean(MixChat);

                        string Text = "";
                        for (int i = 0; i <= SpeechData.Length - 1; i++)
                        {
                            string Phrase = SpeechData[i];
                            if (Phrase.Length > 150)
                            {
                                Phrase.Substring(0, 150);
                            }

                            Text += SpeechData[i] + "\r";
                        }

                        RoomBot.ChatText = Text;
                        RoomBot.LoadRandomSpeech(Text);

                        using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                        BotUserDao.UpdateChat(dbClient, BotId, RoomBot.AutomaticChat, RoomBot.SpeakingInterval, RoomBot.MixSentences, RoomBot.ChatText);

                        break;
                    }
                #endregion

                #region Relax (3)
                case 3:
                    {
                        Bot.BotData.WalkingEnabled = !Bot.BotData.WalkingEnabled;
                        using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                        BotUserDao.UpdateWalkEnabled(dbClient, Bot.BotData.Id, Bot.BotData.WalkingEnabled);
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

                        Room.SendPacket(new DanceComposer(Bot.VirtualId, Bot.DanceId));

                        using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                        BotUserDao.UpdateIsDancing(dbClient, Bot.BotData.Id, Bot.BotData.IsDancing);

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

                        using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                            BotUserDao.UpdateName(dbClient, Bot.BotData.Id, DataString);

                        Room.SendPacket(new UserNameChangeComposer(Bot.BotData.Name, Bot.VirtualId));
                        break;
                    }
                    #endregion
            }
        }
    }
}