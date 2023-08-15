namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;

using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.AI;

internal sealed class ChatGTP : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var bot = room.RoomUserManager.GetBotOrPetByName(parameters[1]);
        if (bot == null || bot.BotData.AiType == BotAIType.ChatGPT)
        {
            return;
        }

        bot.BotData.AiType = BotAIType.ChatGPT;
        bot.BotData.ChatText = WibboEnvironment.GetSettings().GetData<string>("openia.prompt");
        bot.BotData.LoadRandomSpeech(bot.BotData.ChatText);

        bot.BotAI = bot.BotData.GenerateBotAI(bot.VirtualId);
        bot.BotAI.Init(bot.BotData.Id, bot, room);

        var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        BotUserDao.UpdateChatGPT(dbClient, bot.BotData.Id, bot.BotData.ChatText);

        userRoom.SendWhisperChat("ChatGPT vient d'être activé !");
    }
}
