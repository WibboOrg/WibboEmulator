namespace WibboEmulator.Games.Chat.Commands.User.Premium;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal class Vip : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
        {
            return;
        }

        var CurrentEnable = UserRoom.CurrentEffect;
        if (CurrentEnable is 28 or 29 or 30 or 37 or 184 or 77 or 103
            or 40 or 41 or 42 or 43
            or 49 or 50 or 51 or 52
            or 33 or 34 or 35 or 36)
        {
            return;
        }

        if (UserRoom.CurrentEffect == 569)
        {
            UserRoom.ApplyEffect(0);
        }
        else
        {
            UserRoom.ApplyEffect(569);
        }
    }
}
