namespace WibboEmulator.Games.Chat.Commands.User.Inventory;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class EmptyItems : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var emptyAll = parameters.Length > 1 && parameters[1] == "all";

        session.User.InventoryComponent.ClearItems(emptyAll);
        session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("empty.cleared", session.Langue));
    }
}
