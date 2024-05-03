namespace WibboEmulator.Games.Chats.Commands.User.Inventory;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class ConvertMagot : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.IsTrading)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("cmd.troc.not.allowed", session.Language));
            return;
        }

        session.User.InventoryComponent.ConvertMagot();
        userRoom.SendWhisperChat(LanguageManager.TryGetValue("convert.magot", session.Language));
    }
}
