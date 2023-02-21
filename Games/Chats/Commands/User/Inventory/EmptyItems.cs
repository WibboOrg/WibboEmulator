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

        session.User.InventoryComponent.ClearItems(emptyAll);
        session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("empty.cleared", session.Langue));
    }
}
