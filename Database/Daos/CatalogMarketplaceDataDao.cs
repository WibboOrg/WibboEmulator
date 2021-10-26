 internal static void Query8(IQueryAdapter dbClient)
        {
            int Id = 0;
            dbClient.SetQuery("SELECT id FROM catalog_marketplace_data WHERE sprite = " + Item.SpriteId + " LIMIT 1;");
            Id = dbClient.GetInteger();

            if (Id > 0)
            {
                dbClient.RunQuery("UPDATE catalog_marketplace_data SET sold = sold + 1, avgprice = (avgprice + " + Convert.ToInt32(Row["total_price"]) + ") WHERE id = " + Id + " LIMIT 1;");
            }
            else
            {
                dbClient.RunQuery("INSERT INTO catalog_marketplace_data (sprite, sold, avgprice) VALUES ('" + Item.SpriteId + "', '1', '" + Convert.ToInt32(Row["total_price"]) + "')");
            }
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT avgprice FROM catalog_marketplace_data WHERE sprite = @SpriteId LIMIT 1");
            dbClient.AddParameter("SpriteId", SpriteId);
            Row = dbClient.GetRow();
        }
        
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT avgprice FROM catalog_marketplace_data WHERE sprite = '" + SpriteID + "' LIMIT 1");
            num = dbClient.GetInteger();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT sold FROM catalog_marketplace_data WHERE sprite = '" + SpriteID + "' LIMIT 1");
            num2 = dbClient.GetInteger();
        }