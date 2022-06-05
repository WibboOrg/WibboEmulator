
using Wibbo.Communication.Packets.Outgoing.Notifications.NotifCustom;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class NotifTop : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string Message = CommandManager.MergeParams(Params, 1);
            if (string.IsNullOrEmpty(Message))
            {
                return;
            }

            WibboEnvironment.GetGame().GetClientManager().SendMessage(new NotifTopComposer(Message));
        }
    }
}