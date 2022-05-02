using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class DND : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.GetUser().AllowConsoleMessages = !Session.GetUser().AllowConsoleMessages;
            Session.SendWhisper("Tu " + (Session.GetUser().AllowConsoleMessages == true ? "acceptes" : "refuses") + " les messages dans ta console d'amis");
        }
    }
}
