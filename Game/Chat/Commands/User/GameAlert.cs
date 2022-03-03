using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class GameAlert : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            WebClients.WebClient ClientWeb = ButterflyEnvironment.GetGame().GetClientWebManager().GetClientByUserID(UserRoom.UserId);
            if (ClientWeb == null)
            {
                return;
            }

            ClientWeb.ShowGameAlert = !ClientWeb.ShowGameAlert;

            UserRoom.SendWhisperChat(ClientWeb.ShowGameAlert ? "Alerte d'animation activée" : "Alerte d'animation désactivée", true);
        }
    }
}
