using WibboEmulator.Communication.Packets.Outgoing.Notifications;

using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
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
