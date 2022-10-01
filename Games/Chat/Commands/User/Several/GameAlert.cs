using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
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
