using WibboEmulator.Communication.Packets.Outgoing.Inventory.Pets;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Clients;

namespace WibboEmulator.Game.Chat.Commands.Cmd
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
