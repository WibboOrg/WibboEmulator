namespace WibboEmulator.Games.Chats.Commands.User.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class DisableOblique : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters) => room.GameMap.ObliqueDisable = !room.GameMap.ObliqueDisable;
}
