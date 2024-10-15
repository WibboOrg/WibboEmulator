namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;
using System.Text;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Helps;
using WibboEmulator.Games.Rooms;

internal sealed class ShowGuide : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (HelpManager.Count <= 0)
        {
            Session.SendHugeNotification("Aucun guide n'utilise la Guide tool");
        }
        else
        {
            var stringBuilder = new StringBuilder();

            _ = stringBuilder.Append("Guide en service (" + HelpManager.Count + "):\r\r");
            foreach (var entry in HelpManager.GuidesOnDuty)
            {
                var guide = GameClientManager.GetClientByUserID(entry.Key);
                if (guide == null)
                {
                    continue;
                }

                if (entry.Value)
                {
                    _ = stringBuilder.Append("- " + guide.User.Username + " (En service)\r");
                }
                else
                {
                    _ = stringBuilder.Append("- " + guide.User.Username + " (Disponible)\r");
                }
            }

            _ = stringBuilder.Append('\r');
            Session.SendHugeNotification(stringBuilder.ToString());
        }
    }
}
