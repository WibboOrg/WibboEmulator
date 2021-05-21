using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using System;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class About : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            TimeSpan Uptime = DateTime.Now - ButterflyEnvironment.ServerStarted;

            Session.SendPacket(new RoomNotificationComposer("Butterfly Edition Kodamas",
                 "   <b>Credits</b>:\n" +
                 "   Meth0d, Matinmine, Carlos, Super0ca, \n" +
                 "   Mike, Sledmore, Joopie, Tweeny, \n" +
                 "   Kodamas, Leenster, Moogly, Niels, AKllX, rbi0s\n\n" +
                 "   <b>Information sur le serveur</b>:\n" +
                 "   Uptime: " + Uptime.Days + " day(s), " + Uptime.Hours + " hours and " + Uptime.Minutes + " minutes.\n\n"
                    , "staff", "", ""));
        }
    }
}
