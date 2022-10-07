namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Override : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.AllowOverride)
        {
            userRoom.AllowOverride = false;
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("override.disabled", session.Langue));
        }
        else
        {
            userRoom.AllowOverride = true;
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("override.enabled", session.Langue));
        }
    }
}
