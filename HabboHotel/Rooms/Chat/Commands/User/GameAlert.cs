using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.WebClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class GameAlert : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            WebClient ClientWeb = ButterflyEnvironment.GetGame().GetClientWebManager().GetClientByUserID(UserRoom.HabboId);
            if (ClientWeb == null)
            {
                return;
            }

            ClientWeb.ShowGameAlert = !ClientWeb.ShowGameAlert;

            UserRoom.SendWhisperChat(ClientWeb.ShowGameAlert ? "Alerte d'animation activer" : "Alerte d'animation désactiver", true);
        }
    }
}
