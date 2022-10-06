namespace WibboEmulator.Games.Chat.Commands.Staff.Gestion;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class ShutDown : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        var ShutdownTask = new Task(WibboEnvironment.PreformShutDown);
        ShutdownTask.Start();
    }
}
