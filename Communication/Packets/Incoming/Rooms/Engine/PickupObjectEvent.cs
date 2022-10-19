namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;

internal class PickupObjectEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)
    {
        _ = packet.PopInt();
        var itemId = packet.PopInt();

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true))
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
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.7", session.Langue));
            return;
        }

        room.RoomItemHandling.RemoveFurniture(session, item.Id);
        session.User.InventoryComponent.AddItem(item);
        WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.FurniPick, 0);
    }
}
