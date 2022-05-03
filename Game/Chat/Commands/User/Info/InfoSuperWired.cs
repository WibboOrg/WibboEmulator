using Butterfly.Communication.Packets.Outgoing.Notifications;
using Butterfly.Communication.Packets.Outgoing.Custom;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
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
