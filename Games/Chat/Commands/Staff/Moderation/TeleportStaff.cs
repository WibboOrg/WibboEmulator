namespace WibboEmulator.Games.Chat.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class TeleportStaff : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters) => UserRoom.TeleportEnabled = !UserRoom.TeleportEnabled;
}
