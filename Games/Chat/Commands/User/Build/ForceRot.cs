namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class ForceRot : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length != 2)
        {
            return;
        }

        int.TryParse(Params[1], out var result);
        if (result is <= (-1) or >= 7)
        {
            session.GetUser().ForceRot = 0;
        }
        else
        {
            session.GetUser().ForceRot = result;
        }

        session.SendWhisper("Rot: " + result);
    }
}
