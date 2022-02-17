using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Game.Clients;using Butterfly.Game.Items;using Butterfly.Game.Quests;using Butterfly.Game.Rooms;namespace Butterfly.Communication.Packets.Incoming.Structure{    internal class MoveObjectEvent : IPacketEvent    {
        public double Delay => 200;

        public void Parse(Client Session, ClientPacket Packet)        {            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);            if (room == null || !room.CheckRights(Session))
            {
                return;
            }

            Item roomItem = room.GetRoomItemHandler().GetItem(Packet.PopInt());            if (roomItem == null)
            {
                return;
            }

            if (room.RoomData.SellPrice > 0)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("roomsell.error.7", Session.Langue));
                return;
            }            int newX = Packet.PopInt();            int newY = Packet.PopInt();            int newRot = Packet.PopInt();            Packet.PopInt();            if (newX != roomItem.X || newY != roomItem.Y)
            {
                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_MOVE, 0);
            }

            if (newRot != roomItem.Rotation)
            {
                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_ROTATE, 0);
            }

            if (roomItem.Z >= 0.1)
            {
                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_STACK, 0);
            }

            if (!room.GetRoomItemHandler().SetFloorItem(Session, roomItem, newX, newY, newRot, false, false, true))            {                room.SendPacket(new ObjectUpdateComposer(roomItem, room.RoomData.OwnerId));                return;            }        }    }}