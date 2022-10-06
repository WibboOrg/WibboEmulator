namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Quests;

internal class PickupObjectEvent : IPacketEvent
{
    public double Delay => 200;

    public void Parse(GameClient session, ClientPacket packet)
    {
        _ = packet.PopInt();
        var ItemId = packet.PopInt();

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true))
        {
            return;
        }

        var Item = room.GetRoomItemHandler().GetItem(ItemId);
        if (Item == null)
        {
            return;
        }

        if (room.RoomData.SellPrice > 0)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.7", session.Langue));
            return;
        }

        room.GetRoomItemHandler().RemoveFurniture(session, Item.Id);
        session.GetUser().GetInventoryComponent().AddItem(Item);
        WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.FURNI_PICK, 0);
    }
}
