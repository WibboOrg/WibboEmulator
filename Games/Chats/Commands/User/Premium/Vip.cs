namespace WibboEmulator.Games.Chats.Commands.User.Premium;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class Premium : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.Team != TeamType.None || userRoom.InGame || room.IsGameMode)
        {
            return;
        }

        var currentEnable = userRoom.CurrentEffect;
        if (currentEnable is 28 or 29 or 30 or 37 or 184 or 77 or 103
            or 40 or 41 or 42 or 43
            or 49 or 50 or 51 or 52
            or 33 or 34 or 35 or 36)
        {
            return;
        }

        if (userRoom.CurrentEffect == 569)
        {
            userRoom.ApplyEffect(0);
        }
        else
        {
            userRoom.ApplyEffect(569);
        }
    }
}
