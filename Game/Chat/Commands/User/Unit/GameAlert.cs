using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class GameAlert : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.ShowGameAlert = !Session.ShowGameAlert;

            UserRoom.SendWhisperChat(Session.ShowGameAlert ? "Alerte d'animation activée" : "Alerte d'animation désactivée", true);
        }
    }
}
