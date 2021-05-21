using Butterfly.HabboHotel.GameClients;using Butterfly.HabboHotel.Guides;
using System.Collections.Generic;
using System.Text;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class ShowGuide : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            GuideManager guideManager = ButterflyEnvironment.GetGame().GetGuideManager();            if (guideManager.GuidesCount <= 0)            {                Session.SendHugeNotif("Aucun guide n'utilise la Guide tool");            }            else            {                StringBuilder stringBuilder = new StringBuilder();                stringBuilder.Append("Guide en service (" + guideManager.GuidesCount + "):\r\r");                foreach (KeyValuePair<int, bool> entry in guideManager.GuidesOnDuty)                {                    GameClient guide = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(entry.Key);                    if (guide == null)
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