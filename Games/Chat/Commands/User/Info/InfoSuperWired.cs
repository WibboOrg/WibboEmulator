using WibboEmulator.Communication.Packets.Outgoing.Notifications;

using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
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
