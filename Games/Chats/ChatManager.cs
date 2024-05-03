namespace WibboEmulator.Games.Chats;
using System.Data;
using WibboEmulator.Games.Chats.Commands;
using WibboEmulator.Games.Chats.Filter;
using WibboEmulator.Games.Chats.Pets.Commands;
using WibboEmulator.Games.Chats.Styles;

public static class ChatManager
{
    public static void Initialize(IDbConnection dbClient)
    {
        PetCommandManager.Initialize(dbClient);
        ChatStyleManager.Initialize(dbClient);
        CommandManager.Initialize(dbClient);
        WordFilterManager.Initialize(dbClient);
    }
}
