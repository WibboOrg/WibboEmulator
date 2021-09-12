using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.WebClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class GameAlert : IChatCommand
    {
        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }
        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            WebClient ClientWeb = ButterflyEnvironment.GetGame().GetClientWebManager().GetClientByUserID(UserRoom.HabboId);
            if (ClientWeb == null)
            {
                return;
            }

            ClientWeb.ShowGameAlert = !ClientWeb.ShowGameAlert;

            UserRoom.SendWhisperChat(ClientWeb.ShowGameAlert ? "Alerte d'animation activée" : "Alerte d'animation désactivée", true);
        }
    }
}
