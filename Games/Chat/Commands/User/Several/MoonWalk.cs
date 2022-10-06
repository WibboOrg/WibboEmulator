namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal class MoonWalk : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
        {
            return;
        }

        if (UserRoom.InGame)
        {
            return;
        }

        UserRoom.MoonwalkEnabled = !UserRoom.MoonwalkEnabled;
        if (UserRoom.MoonwalkEnabled)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.moonwalk.true", session.Langue));
        }
        else
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.moonwalk.false", session.Langue));
        }
    }
}
