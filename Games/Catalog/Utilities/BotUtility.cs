namespace WibboEmulator.Games.Catalog.Utilities;
using WibboEmulator.Games.Users.Inventory.Bots;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms.AI;
using WibboEmulator.Database.Daos.Bot;

public static class BotUtility
{
    public static Bot CreateBot(ItemData data, int ownerId)
    {
        if (!WibboEnvironment.GetGame().GetCatalog().TryGetBot(data.Id, out var cataBot))
        {
            return null;
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        var Id = BotUserDao.InsertAndGetId(dbClient, ownerId, cataBot.Name, cataBot.Motto, cataBot.Figure, cataBot.Gender);

        var BotData = BotUserDao.GetOne(dbClient, ownerId, Id);

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
