using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Quests;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ApplyDecorationEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || !room.CheckRights(Session, true))
            {
                return;
            }

            Item userItem = Session.GetHabbo().GetInventoryComponent().GetItem(ItemId);
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
                    ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_DECORATION_FLOOR, 0);
                    break;
                case "wallpaper":
                    room.RoomData.Wallpaper = userItem.ExtraData;
                    ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_DECORATION_WALL, 0);
                    break;
                case "landscape":
                    room.RoomData.Landscape = userItem.ExtraData;
                    break;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomDao.UpdateDecoration(dbClient, room.Id, DecorationKey, userItem.ExtraData);

                ItemDao.Delete(dbClient, userItem.Id);
            }

            Session.GetHabbo().GetInventoryComponent().RemoveItem(userItem.Id);
            room.SendPacket(new RoomPropertyComposer(DecorationKey, userItem.ExtraData));
        }
    }
}