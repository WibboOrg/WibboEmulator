namespace WibboEmulator.Games.Chat.Commands.Cmd;
using System.Text;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class UserInfo : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length != 2)
        {
            return;
        }

        var username = Params[1];

        if (string.IsNullOrEmpty(username))
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.userparammissing", session.Langue));
            return;
        }
        var clientByUsername = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(username);
        if (clientByUsername == null || clientByUsername.GetUser() == null)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.useroffline", session.Langue));
            return;
        }

        var user = clientByUsername.GetUser();
        var stringBuilder = new StringBuilder();

        stringBuilder.Append("- Information sur l'utilisateur [" + user.Username + "] -\r");
        stringBuilder.Append("Nom: " + user.Username + "\r");
        stringBuilder.Append("Id: " + user.Id + "\r");
        stringBuilder.Append("Mission: " + user.Motto + "\r");
        stringBuilder.Append("WibboPoints: " + user.WibboPoints + "\r");
        stringBuilder.Append("LimitCoins: " + user.LimitCoins + "\r");
        stringBuilder.Append("Crédits: " + user.Credits + "\r");
        stringBuilder.Append("Win-Win: " + user.AchievementPoints + "\r");
        stringBuilder.Append("Premium: " + ((user.Rank > 1) ? "Oui" : "Non") + "\r");
        stringBuilder.Append("Mazo Score: " + user.MazoHighScore + "\r");
        stringBuilder.Append("Respects: " + user.Respect + "\r");

        stringBuilder.Append("Dans un appart: " + (user.InRoom ? "Oui" : "Non") + "\r");

        if (user.CurrentRoom != null && !user.SpectatorMode)
        {
            stringBuilder.Append("\r - Information sur l'appart  [" + user.CurrentRoom.Id + "] - \r");
            stringBuilder.Append("Propriétaire: " + user.CurrentRoom.RoomData.OwnerName + "\r");
            stringBuilder.Append("Nom: " + user.CurrentRoom.RoomData.Name + "\r");
            stringBuilder.Append("Utilisateurs: " + user.CurrentRoom.UserCount + "/" + user.CurrentRoom.RoomData.UsersMax + "\r");
        }

        if (session.GetUser().HasPermission("perm_god"))
        {
            stringBuilder.Append("\r - Autre information - \r");
            stringBuilder.Append("MachineId: " + clientByUsername.MachineId + "\r");
            stringBuilder.Append("IP Web: " + clientByUsername.GetUser().IP + "\r");
            stringBuilder.Append("IP Emu: " + clientByUsername.GetConnection().GetIp() + "\r");
            stringBuilder.Append("Langue: " + clientByUsername.Langue.ToString() + "\r");
        }

        session.SendNotification(stringBuilder.ToString());
    }
}