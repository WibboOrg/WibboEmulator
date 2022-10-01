using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Catalog;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using System.Data;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class RegenLTD : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (!WibboEnvironment.GetGame().GetCatalog().TryGetPage(984897, out CatalogPage Page))
            {
                return;
            }

            using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            foreach (CatalogItem Item in Page.Items.Values)
            {
                int LimitedStack = Item.LimitedEditionStack;

                for (int LimitedNumber = 1; LimitedNumber < LimitedStack + 1; LimitedNumber++)
                {
                    DataRow Row = ItemDao.GetOneLimitedId(dbClient, LimitedNumber, Item.ItemId);

                    if (Row != null)
                    {
                        continue;
                    }

                    DataRow RowMarketPlace = CatalogMarketplaceOfferDao.GetOneLTD(dbClient, Item.ItemId, LimitedNumber);

                    if (RowMarketPlace != null)
                    {
                        continue;
                    }

                    Item NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetUser(), "", LimitedNumber, LimitedStack);

                    if (NewItem == null)
                    {
                        continue;
                    }

                    if (Session.GetUser().GetInventoryComponent().TryAddItem(NewItem))
                    {
                        Session.SendPacket(new FurniListNotificationComposer(NewItem.Id, 1));
                    }
                }
            }
        }
    }
}