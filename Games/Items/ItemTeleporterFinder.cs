namespace WibboEmulator.Games.Items;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.Rooms;

public static class ItemTeleporterFinder
{
    public static int GetLinkedTele(int teleId)
    {
        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        var row = ItemTeleportDao.GetOne(dbClient, teleId);
        if (row == null)
        {
            return 0;
        }
        else
        {
            return Convert.ToInt32(row[0]);
        }
    }

    public static int GetTeleRoomId(int teleId, Room room)
    {
        if (room == null)
        {
            return 0;
        }

        if (room.RoomItemHandling == null)
        {
            return 0;
        }

        if (room.RoomItemHandling.GetItem(teleId) != null)
        {
            return room.Id;
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        var row = ItemDao.GetOneRoomId(dbClient, teleId);
        if (row == null)
        {
            return 0;
        }
        else
        {
            return Convert.ToInt32(row[0]);
        }
    }

    public static bool IsTeleLinked(int teleId, Room room)
    {
        var linkedTele = GetLinkedTele(teleId);
        if (linkedTele == 0)
        {
            return false;
        }

        var roomItem = room.RoomItemHandling.GetItem(linkedTele);
        return (roomItem != null && (roomItem.GetBaseItem().InteractionType == InteractionType.TELEPORT || roomItem.GetBaseItem().InteractionType == InteractionType.ARROW)) || GetTeleRoomId(linkedTele, room) != 0;
    }
}
