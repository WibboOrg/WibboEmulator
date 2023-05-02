namespace WibboEmulator.Games.Chats.Commands.User.Inventory;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Pets;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class EmptyPets : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        session.User.InventoryComponent.ClearPets();
        session.SendPacket(new PetInventoryComposer(session.User.InventoryComponent.GetPets()));
        userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("empty.cleared", session.Langue));
    }
}
