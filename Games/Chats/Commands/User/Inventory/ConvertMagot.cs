namespace WibboEmulator.Games.Chats.Commands.User.Inventory;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class ConvertMagot : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.IsTrading)
        {
            userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.troc.not.allowed", session.Langue));
            return;
        }

        session.User.InventoryComponent.ConvertMagot();
        session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("convert.magot", session.Langue));
    }
}
