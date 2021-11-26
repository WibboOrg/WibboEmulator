using Butterfly.Game.Clients;
using System.Threading.Tasks;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Shutdown : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Task ShutdownTask = new Task(ButterflyEnvironment.PreformShutDown);
            ShutdownTask.Start();
        }
    }
}
