namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;
using System.Text;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class UserInfo : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var username = parameters[1];

        if (string.IsNullOrEmpty(username))
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("input.userparammissing", Session.Language));
            return;
        }
        var clientByUsername = GameClientManager.GetClientByUsername(username);
        if (clientByUsername == null || clientByUsername.User == null)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("input.useroffline", Session.Language));
            return;
        }

        var user = clientByUsername.User;
        var stringBuilder = new StringBuilder();

        var totalPoints = user.WibboPoints + user.InventoryComponent.GetInventoryPoints();

        _ = stringBuilder.Append("- Information sur l'utilisateur [" + user.Username + "] -\r");
        _ = stringBuilder.Append("Nom: " + user.Username + "\r");
        _ = stringBuilder.Append("Id: " + user.Id + "\r");
        _ = stringBuilder.Append("Mission: " + user.Motto + "\r");
        _ = stringBuilder.Append("WibboPoints: " + totalPoints + "\r");
        _ = stringBuilder.Append("LimitCoins: " + user.LimitCoins + "\r");
        _ = stringBuilder.Append("Crédits: " + user.Credits + "\r");
        _ = stringBuilder.Append("Win-Win: " + user.AchievementPoints + "\r");
        _ = stringBuilder.Append("Premium: " + (user.Rank > 1 ? "Oui" : "Non") + "\r");
        _ = stringBuilder.Append("Mazo Score: " + user.MazoHighScore + "\r");
        _ = stringBuilder.Append("Respects: " + user.Respect + "\r");

        _ = stringBuilder.Append("Dans un appart: " + (user.InRoom ? "Oui" : "Non") + "\r");

        if (user.Room != null && !user.IsSpectator)
        {
            _ = stringBuilder.Append("\r - Information sur l'appart  [" + user.Room.Id + "] - \r");
            _ = stringBuilder.Append("Propriétaire: " + user.Room.RoomData.OwnerName + "\r");
            _ = stringBuilder.Append("Nom: " + user.Room.RoomData.Name + "\r");
            _ = stringBuilder.Append("Utilisateurs: " + user.Room.UserCount + "/" + user.Room.RoomData.UsersMax + "\r");
        }

        if (Session.User.HasPermission("god"))
        {
            _ = stringBuilder.Append("\r - Autre information - \r");
            _ = stringBuilder.Append("IP Web: " + clientByUsername.User.IP + "\r");
            _ = stringBuilder.Append("IP Emu: " + clientByUsername.Connection.Ip + "\r");
            _ = stringBuilder.Append("Langue: " + clientByUsername.Language.ToString() + "\r");
        }

        Session.SendNotification(stringBuilder.ToString());
    }
}
