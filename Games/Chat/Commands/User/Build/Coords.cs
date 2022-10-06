namespace WibboEmulator.Games.Chat.Commands.User.Build;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Coords : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters) => session.SendNotification("X: " + UserRoom.X + " - Y: " + UserRoom.Y + " - Z: " + UserRoom.Z + " - Rot: " + UserRoom.RotBody);
}
