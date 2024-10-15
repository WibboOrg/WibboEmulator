namespace WibboEmulator.Games.Chats.Commands.User.Several;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Override : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.AllowOverride)
        {
            userRoom.AllowOverride = false;
            Session.SendWhisper(LanguageManager.TryGetValue("override.disabled", Session.Language));
        }
        else
        {
            userRoom.AllowOverride = true;
            Session.SendWhisper(LanguageManager.TryGetValue("override.enabled", Session.Language));
        }
    }
}
