namespace WibboEmulator.Games.Chats.Commands.User.Several;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class MoonWalk : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.Team != TeamType.None || userRoom.InGame || room.IsGameMode)
        {
            return;
        }

        userRoom.MoonwalkEnabled = !userRoom.MoonwalkEnabled;
        if (userRoom.MoonwalkEnabled)
        {
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.moonwalk.true", Session.Language));
        }
        else
        {
            Session.SendWhisper(LanguageManager.TryGetValue("cmd.moonwalk.false", Session.Language));
        }
    }
}
