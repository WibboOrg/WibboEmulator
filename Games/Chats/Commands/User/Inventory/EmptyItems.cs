namespace WibboEmulator.Games.Chats.Commands.User.Inventory;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class EmptyItems : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.IsTrading)
        {
            userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.troc.not.allowed", session.Langue));
            return;
        }

        var emptyAll = parameters.Length > 1 && parameters[1] == "all";

        if (emptyAll && !session.User.HasPermission("empty_items_all"))
        {
            emptyAll = false;
        }

        session.User.InventoryComponent.ClearItems(emptyAll);
        userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("empty.cleared", session.Langue));
    }
}
