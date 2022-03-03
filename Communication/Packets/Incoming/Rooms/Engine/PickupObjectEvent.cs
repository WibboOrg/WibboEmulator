using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Quests;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class PickupObjectEvent : IPacketEvent
    {
        public double Delay => 200;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Packet.PopInt();
            int ItemId = Packet.PopInt();

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
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
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("roomsell.error.7", Session.Langue));
                return;
            }

            room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);
            Session.GetUser().GetInventoryComponent().AddItem(Item);
            ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_PICK, 0);
        }
    }
}
