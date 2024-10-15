namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Rooms;

internal sealed class ApplyDecorationEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var itemId = packet.PopInt();

        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(Session, true))
        {
            return;
        }

        var userItem = Session.User.InventoryComponent.GetItem(itemId);
        if (userItem == null)
        {
            return;
        }

        string decorationKey;
        switch (userItem.ItemData.InteractionType)
        {
            case InteractionType.FLOOR:
                decorationKey = "floor";
                break;

            case InteractionType.WALLPAPER:
                decorationKey = "wallpaper";
                break;

            case InteractionType.LANDSCAPE:
                decorationKey = "landscape";
                break;

            default:
                return;
        }

        switch (decorationKey)
        {
            case "floor":
                room.RoomData.Floor = userItem.ExtraData;
                QuestManager.ProgressUserQuest(Session, QuestType.FurniDecorationFloor, 0);
                break;
            case "wallpaper":
                room.RoomData.Wallpaper = userItem.ExtraData;
                QuestManager.ProgressUserQuest(Session, QuestType.FurniDecorationWall, 0);
                break;
            case "landscape":
                room.RoomData.Landscape = userItem.ExtraData;
                break;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            RoomDao.UpdateDecoration(dbClient, room.Id, decorationKey, userItem.ExtraData);
            ItemDao.DeleteById(dbClient, userItem.Id);
        }

        Session.User.InventoryComponent.RemoveItem(userItem.Id);
        room.SendPacket(new RoomPropertyComposer(decorationKey, userItem.ExtraData));
    }
}
