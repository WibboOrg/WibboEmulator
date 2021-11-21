using Butterfly.Game.Clients;
using System.Threading.Tasks;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class Shutdown : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Task ShutdownTask = new Task(ButterflyEnvironment.PreformShutDown);
            //ButterflyEnvironment.PreformShutDown();
            ShutdownTask.Start();
        }
    }
}
