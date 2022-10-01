using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Help;
using System.Text;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class ShowGuide : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            HelpManager guideManager = WibboEnvironment.GetGame().GetHelpManager();
            if (guideManager.GuidesCount <= 0)
            {
                Session.SendHugeNotif("Aucun guide n'utilise la Guide tool");
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.Append("Guide en service (" + guideManager.GuidesCount + "):\r\r");
                foreach (KeyValuePair<int, bool> entry in guideManager.GuidesOnDuty)
                {
                    GameClient guide = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(entry.Key);
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
                Session.SendHugeNotif(stringBuilder.ToString());
            }
        }
    }
}
