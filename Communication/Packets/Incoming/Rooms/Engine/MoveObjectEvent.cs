using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class MoveObjectEvent : IPacketEvent
    {
        public double Delay => 200;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(Session))
            {
                return;
            }

            Item roomItem = room.GetRoomItemHandler().GetItem(Packet.PopInt());
            if (roomItem == null)
            {
                return;
            }

            if (room.RoomData.SellPrice > 0)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.7", Session.Langue));
                return;
            }

            int newX = Packet.PopInt();
            int newY = Packet.PopInt();
            int newRot = Packet.PopInt();
            Packet.PopInt();

            if (newX != roomItem.X || newY != roomItem.Y)
            {
                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_MOVE, 0);
            }

            if (newRot != roomItem.Rotation)
            {
                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_ROTATE, 0);
            }

            if (roomItem.Z >= 0.1)
            {
                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_STACK, 0);
            }

            if (!room.GetRoomItemHandler().SetFloorItem(Session, roomItem, newX, newY, newRot, false, false, true))
            {
                room.SendPacket(new ObjectUpdateComposer(roomItem, room.RoomData.OwnerId));
                return;
            }
        }
    }
}
