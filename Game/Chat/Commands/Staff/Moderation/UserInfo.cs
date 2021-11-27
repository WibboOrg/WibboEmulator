using Butterfly.Game.Clients;
using Butterfly.Game.Users;
using System.Text;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class UserInfo : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            string username = Params[1];

            if (string.IsNullOrEmpty(username))
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.userparammissing", Session.Langue));
                return;
            }
            Client clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(username);
            if (clientByUsername == null || clientByUsername.GetHabbo() == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.useroffline", Session.Langue));
                return;
            }
            User Habbo = clientByUsername.GetHabbo();
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("Nom: " + Habbo.Username + "\r");
            stringBuilder.Append("Id: " + Habbo.Id + "\r");
            stringBuilder.Append("Mission: " + Habbo.Motto + "\r");
            stringBuilder.Append("WibboPoints: " + Habbo.WibboPoints + "\r");
            stringBuilder.Append("Cr�dits: " + Habbo.Credits + "\r");
            stringBuilder.Append("Win-Win: " + Habbo.AchievementPoints + "\r");
            stringBuilder.Append("Premium: " + ((Habbo.Rank > 1) ? "Oui" : "Non") + "\r");
            stringBuilder.Append("Mazo Score: " + Habbo.MazoHighScore + "\r");
            stringBuilder.Append("Respects: " + Habbo.Respect + "\r");
            stringBuilder.Append("Dans un appart: " + ((Habbo.InRoom) ? "Oui" : "Non") + "\r");

            if (Habbo.CurrentRoom != null && !Habbo.SpectatorMode)
            {
                stringBuilder.Append("\r - Information sur l'appart  [" + Habbo.CurrentRoom.Id + "] - \r");
                stringBuilder.Append("Propri�taire: " + Habbo.CurrentRoom.RoomData.OwnerName + "\r");
                stringBuilder.Append("Nom: " + Habbo.CurrentRoom.RoomData.Name + "\r");
                stringBuilder.Append("Utilisateurs: " + Habbo.CurrentRoom.UserCount + "/" + Habbo.CurrentRoom.RoomData.UsersMax + "\r");
            }

            if (Session.GetHabbo().HasFuse("fuse_infouser"))
            {
                stringBuilder.Append("\r - Autre information - \r");
                stringBuilder.Append("MachineId: " + clientByUsername.MachineId + "\r");
                stringBuilder.Append("IP Web: " + clientByUsername.GetHabbo().IP + "\r");
                stringBuilder.Append("IP Emu: " + clientByUsername.GetConnection().GetIp() + "\r");
                stringBuilder.Append("Langue: " + clientByUsername.Langue.ToString() + "\r");
                stringBuilder.Append("Client: " + ((clientByUsername.GetConnection().IsWebSocket) ? "Nitro" : "Flash") + "\r");

                WebClients.WebClient ClientWeb = ButterflyEnvironment.GetGame().GetClientWebManager().GetClientByUserID(Habbo.Id);
                if (ClientWeb != null)
                {
                    stringBuilder.Append("WebSocket: En ligne" + "\r");
                    if (Session.GetHabbo().Rank > 12)
                    {
                        stringBuilder.Append("WebSocket Ip: " + ClientWeb.GetConnection().GetIp() + "\r");
                        stringBuilder.Append("Langue Web: " + ClientWeb.Langue.ToString() + "\r");
                    }
                }
                else
                {
                    stringBuilder.Append("WebSocket: Hors ligne" + "\r");
                }
            }

            Session.SendNotification(stringBuilder.ToString());

        }
    }
}