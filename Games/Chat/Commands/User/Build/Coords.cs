namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Coords : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params) => session.SendNotification("X: " + UserRoom.X + " - Y: " + UserRoom.Y + " - Z: " + UserRoom.Z + " - Rot: " + UserRoom.RotBody);
}
