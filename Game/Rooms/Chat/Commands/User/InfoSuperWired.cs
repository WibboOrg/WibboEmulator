using Butterfly.Communication.Packets.Outgoing.Notifications;
using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Game.Clients;
namespace Butterfly.Game.Rooms.Chat.Commands.Cmd{    internal class InfoSuperWired : IChatCommand    {        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (!Session.GetHabbo().SendWebPacket(new NavigateWebComposer("/forum/sujet/57389")))
            {
                Session.SendPacket(new NuxAlertComposer("habbopages/infosuperwired"));
            }

            return;        }    }}