using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Game.Clients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class PushNotif : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string Message = CommandManager.MergeParams(Params, 1);
            if (string.IsNullOrEmpty(Message))
            {
                return;
            }

            ButterflyEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifTopComposer(Message), Session.Langue);
        }
    }
}