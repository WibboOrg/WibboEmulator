namespace WibboEmulator.Games.Chats.Commands.User.Inventory;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class ConvertMagot : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.IsTrading)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("cmd.troc.not.allowed", Session.Language));
            return;
        }

        Session.User.InventoryComponent.ConvertMagot();
        userRoom.SendWhisperChat(LanguageManager.TryGetValue("convert.magot", Session.Language));
    }
}
