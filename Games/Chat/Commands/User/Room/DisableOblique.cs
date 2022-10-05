namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class DisableOblique : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params) => Room.GetGameMap().ObliqueDisable = !Room.GetGameMap().ObliqueDisable;
}
