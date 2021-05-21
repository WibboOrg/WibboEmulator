using Butterfly.Communication.Packets.Outgoing.Structure;
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
            int OnlineUsersFr = ButterflyEnvironment.GetGame().GetClientManager().OnlineUsersFr;
            int OnlineUsersEn = ButterflyEnvironment.GetGame().GetClientManager().OnlineUsersEn;
            int OnlineUsersBr = ButterflyEnvironment.GetGame().GetClientManager().OnlineUsersBr;

            int OnlineWeb = ButterflyEnvironment.GetGame().GetClientWebManager().Count;
            int RoomCount = ButterflyEnvironment.GetGame().GetRoomManager().Count;


            Session.SendPacket(new RoomNotificationComposer("Butterfly Edition Kodamas",
                 "   <b>Credits</b>:\n" +
                 "   Meth0d, Matinmine, Carlos, Super0ca,\n" +
                 "   Mike, Sledmore, Joopie, Tweeny, \n" +
                 "   Kodamas, Leenster, Moogly, Niels, AKllX, rbi0s\n\n" +
                 "   <b>Information sur le serveur</b>:\n" +
                 "   Joueurs en ligne: " + OnlineUsers + "\n" +
                 "   Joueurs en ligne (FR): " + OnlineUsersFr + "\n" +
                 "   Joueurs en ligne (EN): " + OnlineUsersEn + "\n" +
                 "   Joueurs en ligne (BR): " + OnlineUsersBr + "\n" +
                 "   WebSocket en ligne: " + OnlineWeb + "\n" +
                 "   Appartements actifs: " + RoomCount + "\n" +
                 "   Uptime: " + Uptime.Days + " day(s), " + Uptime.Hours + " hours and " + Uptime.Minutes + " minutes.\n\n"
                    , "staff", "", ""));

        }
    }
}