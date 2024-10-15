namespace WibboEmulator.Games.Chats.Commands.User.Build;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Coords : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters) => userRoom.SendWhisperChat("X: " + userRoom.X + " - Y: " + userRoom.Y + " - Z: " + userRoom.Z + " - Rot: " + userRoom.RotBody);
}
