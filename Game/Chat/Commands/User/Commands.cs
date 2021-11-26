using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Commands : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.SendHugeNotif(ButterflyEnvironment.GetGame().GetChatManager().GetCommands().GetCommandList(Session));
        }
    }
}
