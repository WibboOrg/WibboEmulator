namespace WibboEmulator.Games.Chats.Commands.User.Inventory;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Pets;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class EmptyPets : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        Session.User.InventoryComponent.ClearPets();
        Session.SendPacket(new PetInventoryComposer(Session.User.InventoryComponent.Pets));
        userRoom.SendWhisperChat(LanguageManager.TryGetValue("empty.cleared", Session.Language));
    }
}
