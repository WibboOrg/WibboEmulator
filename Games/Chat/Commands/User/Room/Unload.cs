namespace WibboEmulator.Games.Chat.Commands.User.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Unload : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters) => WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(session.GetUser().CurrentRoom);
}
