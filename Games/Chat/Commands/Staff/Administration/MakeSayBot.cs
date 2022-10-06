namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class MakeSayBot : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length < 3)
        {
            return;
        }

        var username = parameters[1];
        var Bot = Room.GetRoomUserManager().GetBotOrPetByName(username);
        if (Bot == null)
        {
            return;
        }

        var Message = CommandManager.MergeParams(parameters, 2);
        if (string.IsNullOrEmpty(Message))
        {
            return;
        }

        Bot.OnChat(Message, Bot.IsPet ? 0 : 2, false);
    }
}
