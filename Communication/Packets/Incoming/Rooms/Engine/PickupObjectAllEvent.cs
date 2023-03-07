namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;

using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Games.GameClients;

internal sealed class PickupObjectAllEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true))
        {
            return;
        }

        if (room.RoomData.SellPrice > 0)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.7", session.Langue));
            return;
        }

        var itemIds = new List<int>();
        var itemCount = packet.PopInt();

        for (var i = 0; i < itemCount; i++)
        {
            var itemId = packet.PopInt();

            itemIds.Add(itemId);
        }

        session.User.InventoryComponent.AddItemArray(room.RoomItemHandling.RemoveAllFurnitureByIds(session, itemIds));
        session.SendPacket(new FurniListUpdateComposer());
    }
}
