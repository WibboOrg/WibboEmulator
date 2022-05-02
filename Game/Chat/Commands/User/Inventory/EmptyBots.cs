using Butterfly.Communication.Packets.Outgoing.Inventory.Bots;
using Butterfly.Game.Rooms;
using Butterfly.Game.Clients;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class EmptyBots : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.GetUser().GetInventoryComponent().ClearBots();
            Session.SendPacket(new BotInventoryComposer(Session.GetUser().GetInventoryComponent().GetBots()));
            Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("empty.cleared", Session.Langue));
        }
    }
}
