using Butterfly.Communication.Packets.Outgoing.Moderation;
using Butterfly.Game.Clients;
using System;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Info : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            TimeSpan Uptime = DateTime.Now - ButterflyEnvironment.ServerStarted;

            int OnlineUsers = ButterflyEnvironment.GetGame().GetClientManager().Count;

            int RoomCount = ButterflyEnvironment.GetGame().GetRoomManager().Count;


            Session.SendPacket(new BroadcastMessageAlertComposer("<b>Butterfly Edition Wibbo</b>\n\n" +
                 "   <b>Credits</b>:\n" +
                 "   Meth0d, Matinmine, Carlos, Super0ca,\n" +
                 "   Mike, Sledmore, Joopie, Tweeny, \n" +
                 "   JasonDhose, Leenster, Moogly, Niels, AKllX, rbi0s\n\n" +
                 "   <b>Information sur le serveur</b>:\n" +
                 "   Joueurs en ligne: " + OnlineUsers + "\n" +
                 "   Appartements actifs: " + RoomCount + "\n" +
                 "   Uptime: " + Uptime.Days + " jour(s), " + Uptime.Hours + " heure(s) et " + Uptime.Minutes + " minutes.\n\n"));

        }
    }
}