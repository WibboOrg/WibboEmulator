namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Construit : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length != 2)
        {
            return;
        }

        var Heigth = Params[1];
        if (double.TryParse(Heigth, out var Result))
        {
            if (Result is >= 0.01 and <= 10)
            {
                UserRoom.ConstruitEnable = true;
                UserRoom.ConstruitHeigth = Result;
            }
        }

        session.SendWhisper("Construit: " + Result);
    }
}
