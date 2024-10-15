namespace WibboEmulator.Games.Chats.Commands.User.Room;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Pickall : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (room.RoomData.SellPrice > 0)
        {
            session.SendWhisper(LanguageManager.TryGetValue("roomsell.pickall", session.Language));
            return;
        }

        session.User.InventoryComponent.AddItemArray(room.RoomItemHandling.RemoveAllFurnitureToInventory(session));
        session.SendPacket(new FurniListUpdateComposer());
    }
}
