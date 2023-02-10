namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class UnloadEmptyRooms : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters) => WibboEnvironment.GetGame().GetRoomManager().UnloadEmptyRooms();
}
