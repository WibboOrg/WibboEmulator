using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class ShutDown : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Task ShutdownTask = new Task(WibboEnvironment.PreformShutDown);
            ShutdownTask.Start();
        }
    }
}
