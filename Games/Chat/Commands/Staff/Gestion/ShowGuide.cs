namespace WibboEmulator.Games.Chat.Commands.Cmd;
using System.Text;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class ShowGuide : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        var guideManager = WibboEnvironment.GetGame().GetHelpManager();
        if (guideManager.GuidesCount <= 0)
        {
            session.SendHugeNotif("Aucun guide n'utilise la Guide tool");
        }
        else
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("Guide en service (" + guideManager.GuidesCount + "):\r\r");
            foreach (var entry in guideManager.GuidesOnDuty)
            {
                var guide = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(entry.Key);
                if (guide == null)
                {
                    continue;
                }

                if (entry.Value)
                {
                    stringBuilder.Append("- " + guide.GetUser().Username + " (En service)\r");
                }
                else
                {
                    stringBuilder.Append("- " + guide.GetUser().Username + " (Disponible)\r");
                }
            }

            stringBuilder.Append('\r');
            session.SendHugeNotif(stringBuilder.ToString());
        }
    }
}
