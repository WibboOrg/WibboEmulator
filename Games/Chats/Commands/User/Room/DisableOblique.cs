namespace WibboEmulator.Games.Chats.Commands.User.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class DisableOblique : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters) => room.GameMap.ObliqueDisable = !room.GameMap.ObliqueDisable;
}
