using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Items;
using Wibbo.Game.Rooms.AI;
using Wibbo.Game.Users.Inventory.Bots;
using System.Data;

namespace Wibbo.Game.Catalog.Utilities
{
    public static class BotUtility
    {
        public static Bot CreateBot(ItemData Data, int OwnerId)
        {
            if (!WibboEnvironment.GetGame().GetCatalog().TryGetBot(Data.Id, out CatalogBot CataBot))
            {
                return null;
            }

            using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            int Id = BotUserDao.InsertAndGetId(dbClient, OwnerId, CataBot.Name, CataBot.Motto, CataBot.Figure, CataBot.Gender);

            DataRow BotData = BotUserDao.GetOne(dbClient, OwnerId, Id);

            return new Bot(Convert.ToInt32(BotData["id"]), Convert.ToInt32(BotData["user_id"]), Convert.ToString(BotData["name"]), Convert.ToString(BotData["motto"]), Convert.ToString(BotData["look"]), Convert.ToString(BotData["gender"]), false, true, "", 0, false, 0, 0, 0);
        }

        public static BotAIType GetAIFromString(string type)
        {
            switch (type)
            {
                case "pet":
                    return BotAIType.Pet;
                case "generic":
                    return BotAIType.Generic;
                case "copybot":
                    return BotAIType.CopyBot;
                case "roleplaybot":
                    return BotAIType.RoleplayBot;
                case "roleplaypet":
                    return BotAIType.RoleplayPet;
                default:
                    return BotAIType.Generic;
            }
        }
    }
}
