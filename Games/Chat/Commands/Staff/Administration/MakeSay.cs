namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class MakeSay : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length < 3)
        {
            return;
        }

        var username = Params[1];
        var message = CommandManager.MergeParams(Params, 2);

        var roomUserByUserId = session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByName(username);
        if (roomUserByUserId == null)
        {
            return;
        }

        roomUserByUserId.OnChat(message, 0, false);
    }
}
