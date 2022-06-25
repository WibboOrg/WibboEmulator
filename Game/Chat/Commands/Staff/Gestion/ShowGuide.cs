using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Help;
using System.Text;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class ShowGuide : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
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
                    Client guide = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(entry.Key);
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

                stringBuilder.Append("\r");
                Session.SendHugeNotif(stringBuilder.ToString());
            }
        }
    }
}
