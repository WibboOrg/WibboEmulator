namespace WibboEmulator.Games.Chats.Commands.User.Room;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Pickall : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (room.RoomData.SellPrice > 0)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.pickall", session.Langue));
            return;
        }

        session.User.InventoryComponent.AddItemArray(room.RoomItemHandling.RemoveAllFurniture(session));
        session.SendPacket(new FurniListUpdateComposer());
    }
}
