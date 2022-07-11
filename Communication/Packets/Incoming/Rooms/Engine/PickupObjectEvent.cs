using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Items;
using WibboEmulator.Game.Quests;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class PickupObjectEvent : IPacketEvent
    {
        public double Delay => 200;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Packet.PopInt();
            int ItemId = Packet.PopInt();

            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null || !room.CheckRights(Session, true))
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
