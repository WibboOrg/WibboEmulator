namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni.Stickys;

using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal sealed class AddStickyNoteEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session))
        {
            return;
        }

        var id = packet.PopInt();
        var str = packet.PopString();

        var userItem = session.User.InventoryComponent.GetItem(id);
        if (userItem == null || !userItem.IsWallItem || userItem.Data.InteractionType != InteractionType.POSTIT)
        {
            return;
        }

        if (room == null)
        {
            return;
        }

        var wallCoord = ItemWallUtility.WallPositionCheck(":" + str.Split(':')[1]);
        var roomItem = new Item(userItem.Id, room.Id, userItem.BaseItemId, userItem.ExtraData, userItem.Limited, userItem.LimitedStack, 0, 0, 0.0, 0, wallCoord, room);
        if (!room.RoomItemHandling.SetWallItem(session, roomItem))
        {
            return;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            ItemDao.UpdateRoomIdAndUserId(dbClient, id, room.Id, room.RoomData.OwnerId);
        }

        session.User.InventoryComponent.RemoveItem(id);
    }
}
