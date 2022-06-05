using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Rooms;
using System.Data;

namespace Wibbo.Game.Items
{
    public static class ItemTeleporterFinder
    {
        public static int GetLinkedTele(int teleId)
        {
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataRow row = ItemTeleportDao.GetOne(dbClient, teleId);
                if (row == null)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt32(row[0]);
                }
            }
        }

        public static int GetTeleRoomId(int teleId, Room room)
        {
            if (room == null)
            {
                return 0;
            }

            if (room.GetRoomItemHandler() == null)
            {
                return 0;
            }

            if (room.GetRoomItemHandler().GetItem(teleId) != null)
            {
                return room.Id;
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataRow row = ItemDao.GetOneRoomId(dbClient, teleId);
                if (row == null)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt32(row[0]);
                }
            }
        }

        public static bool IsTeleLinked(int teleId, Room room)
        {
            int linkedTele = GetLinkedTele(teleId);
            if (linkedTele == 0)
            {
                return false;
            }

            Item roomItem = room.GetRoomItemHandler().GetItem(linkedTele);
            return roomItem != null && (roomItem.GetBaseItem().InteractionType == InteractionType.TELEPORT || roomItem.GetBaseItem().InteractionType == InteractionType.ARROW) || GetTeleRoomId(linkedTele, room) != 0;
        }
    }
}
