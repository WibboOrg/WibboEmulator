using Wibbo.Communication.Packets.Outgoing.Notifications;

using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class InfoSuperWired : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.SendPacket(new InClientLinkComposer("habbopages/infosuperwired"));

            return;
        }
    }
}
