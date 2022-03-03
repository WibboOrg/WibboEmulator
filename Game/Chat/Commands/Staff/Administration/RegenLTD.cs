using Butterfly.Communication.Packets.Outgoing.Inventory.Furni;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Catalog;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using System.Data;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class RegenLTD : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (!ButterflyEnvironment.GetGame().GetCatalog().TryGetPage(984897, out CatalogPage Page))
            {
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                foreach (CatalogItem Item in Page.Items.Values)
                {
                    int LimitedStack = Item.LimitedEditionStack;

                    for (int LimitedNumber = 1; LimitedNumber < LimitedStack + 1; LimitedNumber++)
                    {
                        DataRow Row = ItemDao.GetOneLimitedId(dbClient, LimitedNumber, LimitedStack, Item.ItemId);

                        if (Row != null)
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
}