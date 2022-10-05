namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Unload : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params) => WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(session.GetUser().CurrentRoom);
}
