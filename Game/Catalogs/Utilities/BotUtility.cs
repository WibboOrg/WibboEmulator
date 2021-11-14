using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Users.Inventory.Bots;
using System;
using System.Data;

namespace Butterfly.Game.Catalog.Utilities
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
                int Id = BotUserDao.InsertAndGetId(dbClient, OwnerId, CataBot.Name, CataBot.Motto, CataBot.Figure, CataBot.Gender);

                DataRow BotData = BotUserDao.GetOne(dbClient, OwnerId, Id);

                return new Bot(Convert.ToInt32(BotData["id"]), Convert.ToInt32(BotData["user_id"]), Convert.ToString(BotData["name"]), Convert.ToString(BotData["motto"]), Convert.ToString(BotData["look"]), Convert.ToString(BotData["gender"]), false, true, "", 0, false, 0, 0, 0);
            }
        }
    }
}