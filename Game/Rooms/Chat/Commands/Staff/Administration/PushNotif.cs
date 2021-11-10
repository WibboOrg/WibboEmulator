using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Game.GameClients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class PushNotif : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
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