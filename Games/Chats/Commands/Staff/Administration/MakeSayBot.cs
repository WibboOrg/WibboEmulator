namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class MakeSayBot : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length < 3)
        {
            return;
        }

        var username = parameters[1];
        var bot = room.RoomUserManager.GetBotOrPetByName(username);
        if (bot == null)
        {
            return;
        }

        var message = CommandManager.MergeParams(parameters, 2);
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        bot.OnChat(message, bot.IsPet ? 0 : 2, false);
    }
}
