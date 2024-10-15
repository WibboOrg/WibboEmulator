namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;

using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class PickupObjectAllEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(Session, true))
        {
            return;
        }

        if (room.RoomData.SellPrice > 0)
        {
            Session.SendNotification(LanguageManager.TryGetValue("roomsell.error.7", Session.Language));
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

        Session.User.InventoryComponent.AddItemArray(room.RoomItemHandling.RemoveFurnitureToInventoryByIds(Session, itemIds));
        Session.SendPacket(new FurniListUpdateComposer());
    }
}
