using Butterfly.Game.GameClients;
using Butterfly.Game.WebClients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class GameAlert : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            WebClients.WebClient ClientWeb = ButterflyEnvironment.GetGame().GetClientWebManager().GetClientByUserID(UserRoom.HabboId);
            if (ClientWeb == null)
            {
                return;
            }

            ClientWeb.ShowGameAlert = !ClientWeb.ShowGameAlert;

            UserRoom.SendWhisperChat(ClientWeb.ShowGameAlert ? "Alerte d'animation activée" : "Alerte d'animation désactivée", true);
        }
    }
}
