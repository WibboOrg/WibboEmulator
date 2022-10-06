namespace WibboEmulator.Games.Chat.Commands.User.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class DisableOblique : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters) => Room.GetGameMap().ObliqueDisable = !Room.GetGameMap().ObliqueDisable;
}
