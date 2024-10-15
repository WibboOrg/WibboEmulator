namespace WibboEmulator.Games.Chats.Commands.User.Room;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Pickall : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (room.RoomData.SellPrice > 0)
        {
            Session.SendWhisper(LanguageManager.TryGetValue("roomsell.pickall", Session.Language));
            return;
        }

        Session.User.InventoryComponent.AddItemArray(room.RoomItemHandling.RemoveAllFurnitureToInventory(Session));
        Session.SendPacket(new FurniListUpdateComposer());
    }
}
