namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class ShutDown : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        var ShutdownTask = new Task(WibboEnvironment.PreformShutDown);
        ShutdownTask.Start();
    }
}
