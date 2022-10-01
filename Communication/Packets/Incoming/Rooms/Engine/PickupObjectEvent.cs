using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class PickupObjectEvent : IPacketEvent
    {
        public double Delay => 200;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Packet.PopInt();
            int ItemId = Packet.PopInt();

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(Session, true))
            {
                return;
            }

            Item Item = room.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
            {
                return;
            }

            if (room.RoomData.SellPrice > 0)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.7", Session.Langue));
                return;
            }

            room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);
            Session.GetUser().GetInventoryComponent().AddItem(Item);
            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_PICK, 0);
        }
    }
}
