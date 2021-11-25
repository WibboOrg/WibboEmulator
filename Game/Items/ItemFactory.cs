using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.User;
using System;
using System.Collections.Generic;

namespace Butterfly.Game.Items
{
    public class ItemFactory
    {
        public static Item CreateSingleItemNullable(ItemData Data, Habbo Habbo, string ExtraData, int LimitedNumber = 0, int LimitedStack = 0)
        {
            if (Data == null)
            {
                throw new InvalidOperationException("Data cannot be null.");
            }

            Item Item = new Item(0, 0, Data.Id, ExtraData, LimitedNumber, LimitedStack, 0, 0, 0, 0, "", null);

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                Item.Id = ItemDao.Insert(dbClient, Data.Id, Habbo.Id, ExtraData);

                if (LimitedNumber > 0)
                {
                    ItemLimitedDao.Insert(dbClient, Item.Id, LimitedNumber, LimitedStack);
                }

                return Item;
            }
        }

        public static Item CreateSingleItem(ItemData Data, Habbo Habbo, string ExtraData, int ItemId, int LimitedNumber = 0, int LimitedStack = 0)
        {
            if (Data == null)
            {
                return null;
            }

            int InsertId = 0;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                InsertId = ItemDao.Insert(dbClient, ItemId, Data.Id, Habbo.Id, ExtraData);

                if (LimitedNumber > 0 && InsertId > 0)
                {
                    ItemLimitedDao.Insert(dbClient, ItemId, LimitedNumber, LimitedStack);
                }
            }

            if (InsertId <= 0)
            {
                return null;
            }

            Item Item = new Item(ItemId, 0, Data.Id, ExtraData, LimitedNumber, LimitedStack, 0, 0, 0, 0, "", null);
            return Item;
        }

        public static List<Item> CreateMultipleItems(ItemData Data, Habbo Habbo, string ExtraData, int Amount)
        {
            if (Data == null) throw new InvalidOperationException("Data cannot be null.");

            List<Item> Items = new List<Item>();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                for (int i = 0; i < Amount; i++)
                {
                    int itemId = ItemDao.Insert(dbClient, Data.Id, Habbo.Id, ExtraData);

                    Item Item = new Item(itemId, 0, Data.Id, ExtraData, 0, 0, 0, 0, 0, 0, "", null);

                    Items.Add(Item);
                }
            }
            return Items;
        }

        public static List<Item> CreateTeleporterItems(ItemData data, Habbo habbo)
        {
            List<Item> Items = new List<Item>();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                int Item1Id = ItemDao.Insert(dbClient, data.Id, habbo.Id, "");
                int Item2Id = ItemDao.Insert(dbClient, data.Id, habbo.Id, Item1Id.ToString());

                Item Item1 = new Item(Item1Id, 0, data.Id, "", 0, 0, 0, 0, 0, 0, "", null);
                Item Item2 = new Item(Item2Id, 0, data.Id, "", 0, 0, 0, 0, 0, 0, "", null);

                ItemTeleportDao.Insert(dbClient, Item1Id, Item2Id);
                ItemTeleportDao.Insert(dbClient, Item2Id, Item1Id);

                Items.Add(Item1);
                Items.Add(Item2);
            }
            return Items;
        }

        public static void CreateMoodlightData(Item Item)
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemMoodlightDao.Insert(dbClient, Item.Id);
            }
        }
    }
}
