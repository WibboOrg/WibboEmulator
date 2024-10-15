namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Teleport : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters) => userRoom.TeleportEnabled = !userRoom.TeleportEnabled;
}
