using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class ShutDown : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Task ShutdownTask = new Task(WibboEnvironment.PreformShutDown);
            ShutdownTask.Start();
        }
    }
}
