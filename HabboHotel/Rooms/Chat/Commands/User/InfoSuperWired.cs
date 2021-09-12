using Butterfly.Communication.Packets.Outgoing.Notifications;
using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.HabboHotel.GameClients;
namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class InfoSuperWired : IChatCommand    {        public string PermissionRequired
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
        }        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)        {            if (!Session.GetHabbo().SendWebPacket(new NavigateWebComposer("/forum/sujet/57389")))
            {
                Session.SendPacket(new NuxAlertComposer("habbopages/infosuperwired"));
            }

            return;        }    }}