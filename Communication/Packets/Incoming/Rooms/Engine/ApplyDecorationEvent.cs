using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class ApplyDecorationEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(Session, true))
            {
                return;
            }

            Item userItem = Session.GetUser().GetInventoryComponent().GetItem(ItemId);
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
                    WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_DECORATION_FLOOR, 0);
                    break;
                case "wallpaper":
                    room.RoomData.Wallpaper = userItem.ExtraData;
                    WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_DECORATION_WALL, 0);
                    break;
                case "landscape":
                    room.RoomData.Landscape = userItem.ExtraData;
                    break;
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomDao.UpdateDecoration(dbClient, room.Id, DecorationKey, userItem.ExtraData);

                ItemDao.Delete(dbClient, userItem.Id);
            }

            Session.GetUser().GetInventoryComponent().RemoveItem(userItem.Id);
            room.SendPacket(new RoomPropertyComposer(DecorationKey, userItem.ExtraData));
        }
    }
}