using Butterfly.Communication.Packets.Outgoing.Notifications;
using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class InfoSuperWired : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (!Session.GetUser().SendWebPacket(new NavigateWebComposer("/forum/sujet/57389")))
            {
                Session.SendPacket(new InClientLinkComposer("habbopages/infosuperwired"));
            }

            return;
        }
    }
}
