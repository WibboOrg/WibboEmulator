using Butterfly.Communication.Packets.Outgoing.Moderation;
using Butterfly.HabboHotel.GameClients;
using System;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class Info : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            TimeSpan Uptime = DateTime.Now - ButterflyEnvironment.ServerStarted;

            int OnlineUsers = ButterflyEnvironment.GetGame().GetClientManager().Count;
            int OnlineNitroUsers = ButterflyEnvironment.GetGame().GetClientManager().OnlineNitroUsers;
            int OnlineFlashUsers = OnlineUsers - OnlineNitroUsers;
            int OnlineUsersFr = ButterflyEnvironment.GetGame().GetClientManager().OnlineUsersFr;
            int OnlineUsersEn = ButterflyEnvironment.GetGame().GetClientManager().OnlineUsersEn;
            int OnlineUsersBr = ButterflyEnvironment.GetGame().GetClientManager().OnlineUsersBr;

            int OnlineWeb = ButterflyEnvironment.GetGame().GetClientWebManager().Count;
            int RoomCount = ButterflyEnvironment.GetGame().GetRoomManager().Count;


            Session.SendPacket(new BroadcastMessageAlertComposer("<b>Butterfly Edition Wibbo</b>\n\n" +
                 "   <b>Credits</b>:\n" +
                 "   Meth0d, Matinmine, Carlos, Super0ca,\n" +
                 "   Mike, Sledmore, Joopie, Tweeny, \n" +
                 "   JasonDhose, Leenster, Moogly, Niels, AKllX, rbi0s\n\n" +
                 "   <b>Information sur le serveur</b>:\n" +
                 "   Joueurs en ligne total: " + OnlineUsers + "\n" +
                 "   Joueurs en ligne sur Flash: " + OnlineFlashUsers + "\n" +
                 "   Joueurs en ligne sur Nitro: " + OnlineNitroUsers + "\n" +
                 //"   Joueurs en ligne (FR): " + OnlineUsersFr + "\n" +
                 //"   Joueurs en ligne (EN): " + OnlineUsersEn + "\n" +
                 //"   Joueurs en ligne (BR): " + OnlineUsersBr + "\n" +
                 //"   WebSocket en ligne: " + OnlineWeb + "\n" +
                 "   Appartements actifs: " + RoomCount + "\n" +
                 "   Uptime: " + Uptime.Days + " jour(s), " + Uptime.Hours + " heure(s) et " + Uptime.Minutes + " minutes.\n\n"));

        }
    }
}