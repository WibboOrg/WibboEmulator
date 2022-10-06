namespace WibboEmulator.Games.Chat.Commands.User.Inventory;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class EmptyItems : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        var EmptyAll = parameters.Length > 1 && parameters[1] == "all";

        session.GetUser().GetInventoryComponent().ClearItems(EmptyAll);
        session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("empty.cleared", session.Langue));
    }
}
