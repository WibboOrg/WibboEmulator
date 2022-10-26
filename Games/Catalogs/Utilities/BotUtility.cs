namespace WibboEmulator.Games.Catalogs.Utilities;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms.AI;
using WibboEmulator.Games.Users.Inventory.Bots;

public static class BotUtility
{
    public static Bot CreateBot(ItemData data, int ownerId)
    {
        if (!WibboEnvironment.GetGame().GetCatalog().TryGetBot(data.Id, out var cataBot))
        {
            return null;
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        var id = BotUserDao.InsertAndGetId(dbClient, ownerId, cataBot.Name, cataBot.Motto, cataBot.Figure, cataBot.Gender);

        var botData = BotUserDao.GetOne(dbClient, ownerId, id);

        return new Bot(Convert.ToInt32(botData["id"]), Convert.ToInt32(botData["user_id"]), Convert.ToString(botData["name"]), Convert.ToString(botData["motto"]), Convert.ToString(botData["look"]), Convert.ToString(botData["gender"]), false, true, "", 0, false, 0, 0, 0);
    }

    public static BotAIType GetAIFromString(string type) => type switch
    {
        "pet" => BotAIType.Pet,
        "generic" => BotAIType.Generic,
        "copybot" => BotAIType.CopyBot,
        "roleplaybot" => BotAIType.RoleplayBot,
        "roleplaypet" => BotAIType.RoleplayPet,
        _ => BotAIType.Generic,
    };
}
