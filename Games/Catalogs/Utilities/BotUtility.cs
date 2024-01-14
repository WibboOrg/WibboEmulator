namespace WibboEmulator.Games.Catalogs.Utilities;
using WibboEmulator.Database.Daos.Bot;
using System.Data;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms.AI;
using WibboEmulator.Games.Users.Inventory.Bots;

public static class BotUtility
{
    public static Bot CreateBot(IDbConnection dbClient, ItemData data, int ownerId)
    {
        if (!WibboEnvironment.GetGame().GetCatalog().TryGetBot(data.Id, out var cataBot))
        {
            return null;
        }

        var id = BotUserDao.InsertAndGetId(dbClient, ownerId, cataBot.Name, cataBot.Motto, cataBot.Figure, cataBot.Gender);

        return new Bot(id, ownerId, cataBot.Name, cataBot.Motto, cataBot.Figure, cataBot.Gender, false, true, "", 0, false, 0, 0, 0, BotAIType.Generic);
    }

    public static BotAIType GetAIFromString(string type) => type switch
    {
        "pet" => BotAIType.Pet,
        "generic" => BotAIType.Generic,
        "roleplaybot" => BotAIType.RoleplayBot,
        "roleplaypet" => BotAIType.RoleplayPet,
        "chatgpt" => BotAIType.ChatGPT,
        _ => BotAIType.Generic,
    };
}
