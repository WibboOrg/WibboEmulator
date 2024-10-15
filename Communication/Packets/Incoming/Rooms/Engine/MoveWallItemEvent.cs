namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.Rooms;

internal sealed class MoveWallItemEvent : IPacketEvent
{
    public double Delay => 200;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(Session))
        {
            return;
        }

        var id = packet.PopInt();
        var str = packet.PopString();

        var roomItem = room.RoomItemHandling.GetItem(id);
        if (roomItem == null)
        {
            return;
        }

        if (room.RoomData.SellPrice > 0)
        {
            Session.SendNotification(LanguageManager.TryGetValue("roomsell.error.7", Session.Language));
            return;
        }

        var wallCoordinate = ItemWallUtility.WallPositionCheck(":" + str.Split(':')[1]);
        roomItem.WallCoord = wallCoordinate;
        room.RoomItemHandling.UpdateItem(roomItem);

        room.SendPacket(new ItemUpdateComposer(roomItem, room.RoomData.OwnerId));

    }
}
