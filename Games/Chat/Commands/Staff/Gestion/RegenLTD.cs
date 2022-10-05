namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal class RegenLTD : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (!WibboEnvironment.GetGame().GetCatalog().TryGetPage(984897, out var Page))
        {
            return;
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        foreach (var Item in Page.Items.Values)
        {
            var LimitedStack = Item.LimitedEditionStack;

            for (var LimitedNumber = 1; LimitedNumber < LimitedStack + 1; LimitedNumber++)
            {
                var Row = ItemDao.GetOneLimitedId(dbClient, LimitedNumber, Item.ItemId);

                if (Row != null)
                {
                    continue;
                }

                var RowMarketPlace = CatalogMarketplaceOfferDao.GetOneLTD(dbClient, Item.ItemId, LimitedNumber);

                if (RowMarketPlace != null)
                {
                    continue;
                }

                var NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, session.GetUser(), "", LimitedNumber, LimitedStack);

                if (NewItem == null)
                {
                    continue;
                }

                if (session.GetUser().GetInventoryComponent().TryAddItem(NewItem))
                {
                    session.SendPacket(new FurniListNotificationComposer(NewItem.Id, 1));
                }
            }
        }
    }
}