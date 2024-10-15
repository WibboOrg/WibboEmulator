namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;

using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class PickupObjectAllEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true))
        {
            return;
        }

        if (room.RoomData.SellPrice > 0)
        {
            session.SendNotification(LanguageManager.TryGetValue("roomsell.error.7", session.Language));
            return;
        }

        var itemIds = new List<int>();
        var itemCount = packet.PopInt();

        itemCount = itemCount > 100 ? 100 : itemCount;

        for (var i = 0; i < itemCount; i++)
        {
            var itemId = packet.PopInt();

            itemIds.Add(itemId);
        }

        session.User.InventoryComponent.AddItemArray(room.RoomItemHandling.RemoveFurnitureToInventoryByIds(session, itemIds));
        session.SendPacket(new FurniListUpdateComposer());
    }
}
