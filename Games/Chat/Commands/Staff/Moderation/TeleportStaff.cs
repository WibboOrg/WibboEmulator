namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class TeleportStaff : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params) => UserRoom.TeleportEnabled = !UserRoom.TeleportEnabled;
}
