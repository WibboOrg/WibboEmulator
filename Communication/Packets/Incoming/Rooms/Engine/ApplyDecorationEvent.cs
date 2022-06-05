using Wibbo.Communication.Packets.Outgoing.Rooms.Engine;
using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Clients;
using Wibbo.Game.Items;
using Wibbo.Game.Quests;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class ApplyDecorationEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();

            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null || !room.CheckRights(Session, true))
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