namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class SetZ : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length != 2)
        {
            return;
        }

        var Heigth = Params[1];
        if (!double.TryParse(Heigth, out var Result))
        {
            return;
        }

        if (Result < -100)
        {
            Result = 0;
        }

        if (Result > 100)
        {
            Result = 100;
        }

        UserRoom.ConstruitZMode = true;
        UserRoom.ConstruitHeigth = Result;

        session.SendWhisper("SetZ: " + Result);

        if (Result >= 0)
        {
            session.SendPacket(Room.GetGameMap().Model.SetHeightMap((Result > 63) ? 63 : Result));
        }
    }
}
