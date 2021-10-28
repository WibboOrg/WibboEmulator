using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Users.Inventory.Bots;
using System;
using System.Data;

namespace Butterfly.HabboHotel.Catalog.Utilities
{
    public static class BotUtility
    {
        public static Bot CreateBot(ItemData Data, int OwnerId)
        {
            if (!ButterflyEnvironment.GetGame().GetCatalog().TryGetBot(Data.Id, out CatalogBot CataBot))
            {
                return null;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO bots (user_id,name,motto,look,gender) VALUES ('" + OwnerId + "', '" + CataBot.Name + "', '" + CataBot.Motto + "', '" + CataBot.Figure + "', '" + CataBot.Gender + "')");
                int Id = Convert.ToInt32(dbClient.InsertQuery());

                dbClient.SetQuery("SELECT id,user_id,name,motto,look,gender FROM bots WHERE user_id = '" + OwnerId + "' AND id = '" + Id + "' LIMIT 1");
                DataRow BotData = dbClient.GetRow();

                return new Bot(Convert.ToInt32(BotData["id"]), Convert.ToInt32(BotData["user_id"]), Convert.ToString(BotData["name"]), Convert.ToString(BotData["motto"]), Convert.ToString(BotData["look"]), Convert.ToString(BotData["gender"]), false, true, "", 0, false, 0, 0, 0);
            }
        }
    }
}