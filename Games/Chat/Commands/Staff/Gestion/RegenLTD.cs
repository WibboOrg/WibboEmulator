namespace WibboEmulator.Games.Chat.Commands.Staff.Gestion;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal class RegenLTD : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (!WibboEnvironment.GetGame().GetCatalog().TryGetPage(984897, out var page))
        {
            return;
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        foreach (var item in page.Items.Values)
        {
            var limitedStack = item.LimitedEditionStack;

            for (var limitedNumber = 1; limitedNumber < limitedStack + 1; limitedNumber++)
            {
                var row = ItemDao.GetOneLimitedId(dbClient, limitedNumber, item.ItemId);

                if (row != null)
                {
                    continue;
                }

                var rowMarketPlace = CatalogMarketplaceOfferDao.GetOneLTD(dbClient, item.ItemId, limitedNumber);

                if (rowMarketPlace != null)
                {
                    continue;
                }

                var newItem = ItemFactory.CreateSingleItemNullable(item.Data, session.GetUser(), "", limitedNumber, limitedStack);

                if (newItem == null)
                {
                    continue;
                }

                if (session.GetUser().GetInventoryComponent().TryAddItem(newItem))
                {
                    session.SendPacket(new FurniListNotificationComposer(newItem.Id, 1));
                }
            }
        }
    }
}
