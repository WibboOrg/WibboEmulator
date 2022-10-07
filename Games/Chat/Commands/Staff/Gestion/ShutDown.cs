namespace WibboEmulator.Games.Chat.Commands.Staff.Gestion;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class ShutDown : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var shutdownTask = new Task(WibboEnvironment.PreformShutDown);
        shutdownTask.Start();
    }
}
