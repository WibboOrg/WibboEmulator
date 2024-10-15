namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class ShutDown : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters) => _ = Task.Run(WibboEnvironment.PerformShutDown);
}

