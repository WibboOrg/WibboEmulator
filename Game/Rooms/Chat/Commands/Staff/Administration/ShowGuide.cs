using Butterfly.Game.Clients;using Butterfly.Game.Help;
using System.Collections.Generic;
using System.Text;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd{    internal class ShowGuide : IChatCommand    {        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)        {            HelpManager guideManager = ButterflyEnvironment.GetGame().GetHelpManager();            if (guideManager.GuidesCount <= 0)            {                Session.SendHugeNotif("Aucun guide n'utilise la Guide tool");            }            else            {                StringBuilder stringBuilder = new StringBuilder();                stringBuilder.Append("Guide en service (" + guideManager.GuidesCount + "):\r\r");                foreach (KeyValuePair<int, bool> entry in guideManager.GuidesOnDuty)                {                    Client guide = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(entry.Key);                    if (guide == null)
                    {
                        continue;
                    }

                    if (entry.Value)
                    {
                        stringBuilder.Append("- " + guide.GetHabbo().Username + " (En service)\r");
                    }
                    else
                    {
                        stringBuilder.Append("- " + guide.GetHabbo().Username + " (Disponible)\r");
                    }
                }                stringBuilder.Append("\r");                Session.SendHugeNotif(stringBuilder.ToString());            }        }    }}