namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class MoonWalk : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.Team != TeamType.None || userRoom.InGame || room.IsGameMode)
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
