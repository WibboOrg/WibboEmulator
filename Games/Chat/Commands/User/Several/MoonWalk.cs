namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal class MoonWalk : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.Team != TeamType.None || userRoom.InGame)
        {
            return;
        }

        if (userRoom.InGame)
        {
            return;
        }

        userRoom.MoonwalkEnabled = !userRoom.MoonwalkEnabled;
        if (userRoom.MoonwalkEnabled)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.moonwalk.true", session.Langue));
        }
        else
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.moonwalk.false", session.Langue));
        }
    }
}
