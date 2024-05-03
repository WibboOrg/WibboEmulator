namespace WibboEmulator.Games.Chats.Commands.User.Several;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Override : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.AllowOverride)
        {
            userRoom.AllowOverride = false;
            session.SendWhisper(LanguageManager.TryGetValue("override.disabled", session.Language));
        }
        else
        {
            userRoom.AllowOverride = true;
            session.SendWhisper(LanguageManager.TryGetValue("override.enabled", session.Language));
        }
    }
}
