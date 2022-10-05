namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Quests;

internal class ApplyDecorationEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var ItemId = Packet.PopInt();

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true))
        {
            return;
        }

        var userItem = session.GetUser().GetInventoryComponent().GetItem(ItemId);
        if (userItem == null)
        {
            return;
        }

        string DecorationKey;
        switch (userItem.GetBaseItem().InteractionType)
        {
            case InteractionType.FLOOR:
                DecorationKey = "floor";
                break;

            case InteractionType.WALLPAPER:
                DecorationKey = "wallpaper";
                break;

            case InteractionType.LANDSCAPE:
                DecorationKey = "landscape";
                break;

            default:
                return;
        }

        switch (DecorationKey)
        {
            case "floor":
                room.RoomData.Floor = userItem.ExtraData;
                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.FURNI_DECORATION_FLOOR, 0);
                break;
            case "wallpaper":
                room.RoomData.Wallpaper = userItem.ExtraData;
                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.FURNI_DECORATION_WALL, 0);
                break;
            case "landscape":
                room.RoomData.Landscape = userItem.ExtraData;
                break;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            RoomDao.UpdateDecoration(dbClient, room.Id, DecorationKey, userItem.ExtraData);

            ItemDao.Delete(dbClient, userItem.Id);
        }

        session.GetUser().GetInventoryComponent().RemoveItem(userItem.Id);
        room.SendPacket(new RoomPropertyComposer(DecorationKey, userItem.ExtraData));
    }
}