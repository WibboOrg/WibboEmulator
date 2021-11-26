using Butterfly.Communication.Packets.Outgoing.Inventory.Bots;
using Butterfly.Game.Rooms;
using Butterfly.Game.Clients;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class EmptyBots : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.GetHabbo().GetInventoryComponent().ClearBots();
            Session.SendPacket(new BotInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetBots()));
            Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("empty.cleared", Session.Langue));
        }
    }
}
