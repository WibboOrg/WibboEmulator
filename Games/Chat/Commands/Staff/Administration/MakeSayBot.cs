namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class MakeSayBot : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length < 3)
        {
            return;
        }

        var username = Params[1];
        var Bot = Room.GetRoomUserManager().GetBotOrPetByName(username);
        if (Bot == null)
        {
            return;
        }

        var Message = CommandManager.MergeParams(Params, 2);
        if (string.IsNullOrEmpty(Message))
        {
            return;
        }

        Bot.OnChat(Message, Bot.IsPet ? 0 : 2, false);
    }
}
