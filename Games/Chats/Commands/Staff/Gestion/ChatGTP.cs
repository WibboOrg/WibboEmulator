namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;

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

        var bot = room.RoomUserManager.GetBotByName(parameters[1]);
        if (bot == null)
        {
            return;
        }

        bot.BotData.AiType = BotAIType.OpenIA;
        bot.BotAI = bot.BotData.GenerateBotAI(bot.VirtualId);
        bot.BotAI.Init(bot.BotData.Id, bot, room);

        userRoom.SendWhisperChat("ChatGPT vient d'être activé !");
    }
}
