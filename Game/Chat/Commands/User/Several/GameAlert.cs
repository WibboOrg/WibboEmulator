using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class GameAlert : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.ShowGameAlert = !Session.ShowGameAlert;

            Session.SendWhisper(Session.ShowGameAlert ? "Alerte d'animation activée" : "Alerte d'animation désactivée", true);
        }
    }
}
