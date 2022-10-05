namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.AI;

internal class SaveBotActionEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (!session.GetUser().InRoom)
        {
            return;
        }

        var Room = session.GetUser().CurrentRoom;
        if (Room == null || !Room.CheckRights(session, true))
        {
            return;
        }

        var BotId = Packet.PopInt();
        var ActionId = Packet.PopInt();
        var DataString = Packet.PopString();

        if (BotId <= 0)
        {
            return;
        }

        if (ActionId is < 1 or > 5)
        {
            return;
        }

        if (!Room.GetRoomUserManager().TryGetBot(BotId, out var Bot))
        {
            return;
        }

        var RoomBot = Bot.BotData;
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
            case 1:
            {
                //Change the defaults
                Bot.BotData.Look = session.GetUser().Look;
                Bot.BotData.Gender = session.GetUser().Gender;

                Room.SendPacket(new UserChangeComposer(Bot));

                using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                BotUserDao.UpdateLookGender(dbClient, Bot.BotData.Id, session.GetUser().Gender, session.GetUser().Look);
                break;
            }

            case 2:
            {

                var ConfigData = DataString.Split(new string[]
                {
                        ";#;"
                }, StringSplitOptions.None);

                var SpeechData = ConfigData[0].Split(new char[]
                {
                        '\r',
                        '\n'
                }, StringSplitOptions.RemoveEmptyEntries);

                var AutomaticChat = Convert.ToString(ConfigData[1]);
                var SpeakingIntervalIsInt = int.TryParse(ConfigData[2], out var SpeakingInterval);
                var MixChat = Convert.ToString(ConfigData[3]);

                if (SpeakingInterval <= 0 || SpeakingInterval < 7 || !SpeakingIntervalIsInt)
                {
                    SpeakingInterval = 7;
                }

                RoomBot.AutomaticChat = Convert.ToBoolean(AutomaticChat);
                RoomBot.SpeakingInterval = SpeakingInterval;
                RoomBot.MixSentences = Convert.ToBoolean(MixChat);

                var text = "";
                for (var i = 0; i <= SpeechData.Length - 1; i++)
                {
                    var phrase = SpeechData[i];
                    if (phrase.Length > 150)
                    {
                        phrase = phrase[..150];
                    }

                    text += phrase[i] + "\r";
                }

                RoomBot.ChatText = text;
                RoomBot.LoadRandomSpeech(text);

                using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                BotUserDao.UpdateChat(dbClient, BotId, RoomBot.AutomaticChat, RoomBot.SpeakingInterval, RoomBot.MixSentences, RoomBot.ChatText);

                break;
            }

            case 3:
            {
                Bot.BotData.WalkingEnabled = !Bot.BotData.WalkingEnabled;
                using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                BotUserDao.UpdateWalkEnabled(dbClient, Bot.BotData.Id, Bot.BotData.WalkingEnabled);
                break;
            }

            case 4:
            {
                if (Bot.DanceId > 0)
                {
                    Bot.DanceId = 0;
                    Bot.BotData.IsDancing = false;
                }
                else
                {
                    Bot.DanceId = WibboEnvironment.GetRandomNumber(1, 4);
                    Bot.BotData.IsDancing = true;
                }

                Room.SendPacket(new DanceComposer(Bot.VirtualId, Bot.DanceId));

                using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                BotUserDao.UpdateIsDancing(dbClient, Bot.BotData.Id, Bot.BotData.IsDancing);

                break;
            }

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

                using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    BotUserDao.UpdateName(dbClient, Bot.BotData.Id, DataString);
                }

                Room.SendPacket(new UserNameChangeComposer(Bot.BotData.Name, Bot.VirtualId));
                break;
            }
        }
    }
}
