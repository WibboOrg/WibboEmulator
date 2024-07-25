namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;

using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal sealed class RegenLTD : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        using var dbClient = DatabaseManager.Connection;

        var ltdList = CatalogItemLimitedDao.GetAll(dbClient);

        foreach (var item in ltdList)
        {
            var limitedSells = item.LimitedSells;
            var limitedStack = item.LimitedStack;

            for (var limitedNumber = 1; limitedNumber < limitedSells + 1; limitedNumber++)
            {
                var limitedId = ItemDao.GetOneLimitedId(dbClient, limitedNumber, item.ItemId);

                if (limitedId > 0)
                {
                    continue;
                }

                var marketPlaceId = CatalogMarketplaceOfferDao.GetOneLTD(dbClient, item.ItemId, limitedNumber);

                if (marketPlaceId > 0)
                {
                    continue;
                }

                if (!ItemManager.GetItem(item.ItemId, out var itemData))
                {
                    continue;
                }

                var newItem = ItemFactory.CreateSingleItemNullable(dbClient, itemData, session.User, "", limitedNumber, limitedStack);

                if (newItem == null)
                {
                    continue;
                }

                session.User.InventoryComponent.TryAddItem(newItem);
            }
        }
    }
}
