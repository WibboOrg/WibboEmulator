using Wibbo.Communication.Packets.Outgoing.Inventory.Pets;
using Wibbo.Game.Rooms;
using Wibbo.Game.Clients;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class EmptyPets : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.GetUser().GetInventoryComponent().ClearPets();
            Session.SendPacket(new PetInventoryComposer(Session.GetUser().GetInventoryComponent().GetPets()));
            Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("empty.cleared", Session.Langue));

        }
    }
}
