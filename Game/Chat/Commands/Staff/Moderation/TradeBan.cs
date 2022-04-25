using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class TradeBan : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {

            if (Params.Length == 1)
            {
                UserRoom.SendWhisperChat("Il semble que vous ayez oublié des valeurs? (jours ou pseudonyme)");
                return;
            }
        }
    }
}
