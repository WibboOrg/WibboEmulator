using Butterfly.HabboHotel.GameClients;
using System.Threading.Tasks;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class Shutdown : IChatCommand
    {
        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }
        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            Task ShutdownTask = new Task(ButterflyEnvironment.PreformShutDown);
            //ButterflyEnvironment.PreformShutDown();
            ShutdownTask.Start();
        }
    }
}
