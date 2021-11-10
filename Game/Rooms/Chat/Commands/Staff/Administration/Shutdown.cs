using Butterfly.Game.GameClients;
using System.Threading.Tasks;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class Shutdown : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Task ShutdownTask = new Task(ButterflyEnvironment.PreformShutDown);
            //ButterflyEnvironment.PreformShutDown();
            ShutdownTask.Start();
        }
    }
}
