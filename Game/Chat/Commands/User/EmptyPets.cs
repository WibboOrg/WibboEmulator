using Butterfly.Communication.Packets.Outgoing.Inventory.Pets;
using Butterfly.Game.Rooms;
using Butterfly.Game.Clients;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class EmptyPets : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.GetHabbo().GetInventoryComponent().ClearPets();
            Session.SendPacket(new PetInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetPets()));
            Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("empty.cleared", Session.Langue));

        }
    }
}
