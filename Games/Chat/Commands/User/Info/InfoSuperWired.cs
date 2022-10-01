using WibboEmulator.Communication.Packets.Outgoing.Notifications;

using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class InfoSuperWired : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.SendPacket(new InClientLinkComposer("habbopages/infosuperwired"));

            return;
        }
    }
}
