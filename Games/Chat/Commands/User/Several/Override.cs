namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Override : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (UserRoom.AllowOverride)
        {
            UserRoom.AllowOverride = false;
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("override.disabled", session.Langue));
        }
        else
        {
            UserRoom.AllowOverride = true;
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("override.enabled", session.Langue));
        }
    }
}
