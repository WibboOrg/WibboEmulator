namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class EmptyItems : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        var EmptyAll = Params.Length > 1 && Params[1] == "all";

        session.GetUser().GetInventoryComponent().ClearItems(EmptyAll);
        session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("empty.cleared", session.Langue));
    }
}
