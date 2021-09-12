using Butterfly.HabboHotel.GameClients;
using System;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class About : IChatCommand
    {
        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }
        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            TimeSpan Uptime = DateTime.Now - ButterflyEnvironment.ServerStarted;

            Session.SendHugeNotif("<b>Butterfly Edition Wibbo</b>\n\n" +
                "   <b>Credits</b>:\n" +
                "   Meth0d, Matinmine, Carlos, Super0ca,\n" +
                "   Mike, Sledmore, Joopie, Tweeny, \n" +
                "   JasonDhose, Leenster, Moogly, Niels, AKllX, rbi0s\n\n" +
                "   <b>Information sur le serveur</b>:\n" +
                "   Uptime: " + Uptime.Days + " jour(s), " + Uptime.Hours + " heure(s) et " + Uptime.Minutes + " minutes.\n\n");
        }
    }
}
