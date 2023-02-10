namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;
using System.Text;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class ShowGuide : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var guideManager = WibboEnvironment.GetGame().GetHelpManager();
        if (guideManager.GuidesCount <= 0)
        {
            session.SendHugeNotif("Aucun guide n'utilise la Guide tool");
        }
        else
        {
            var stringBuilder = new StringBuilder();

            _ = stringBuilder.Append("Guide en service (" + guideManager.GuidesCount + "):\r\r");
            foreach (var entry in guideManager.GuidesOnDuty)
            {
                var guide = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(entry.Key);
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
            session.SendHugeNotif(stringBuilder.ToString());
        }
    }
}
