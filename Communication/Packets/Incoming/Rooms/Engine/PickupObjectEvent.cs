namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;

using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Rooms;

internal sealed class PickupObjectEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        _ = packet.PopInt();
        var itemId = packet.PopInt();

        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(Session, true))
        {
            return;
        }

        var item = room.RoomItemHandling.GetItem(itemId);
        if (item == null)
        {
            return;
        }

        if (room.RoomData.SellPrice > 0)
        {
            Session.SendNotification(LanguageManager.TryGetValue("roomsell.error.7", Session.Language));
            return;
        }

        using var dbClient = DatabaseManager.Connection;

        room.RoomItemHandling.RemoveFurniture(Session, item.Id);
        Session.User.InventoryComponent.AddItem(dbClient, item);
        QuestManager.ProgressUserQuest(Session, QuestType.FurniPick, 0);
    }
}
