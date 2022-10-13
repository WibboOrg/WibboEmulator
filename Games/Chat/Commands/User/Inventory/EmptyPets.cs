namespace WibboEmulator.Games.Chat.Commands.User.Inventory;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Pets;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class EmptyPets : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        session.GetUser().InventoryComponent.ClearPets();
        session.SendPacket(new PetInventoryComposer(session.GetUser().InventoryComponent.GetPets()));
        session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("empty.cleared", session.Langue));

    }
}
