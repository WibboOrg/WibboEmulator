using WibboEmulator.Communication.Packets.Outgoing.Inventory.Bots;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Clients;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class EmptyBots : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.GetUser().GetInventoryComponent().ClearBots();
            Session.SendPacket(new BotInventoryComposer(Session.GetUser().GetInventoryComponent().GetBots()));
            Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("empty.cleared", Session.Langue));
        }
    }
}
