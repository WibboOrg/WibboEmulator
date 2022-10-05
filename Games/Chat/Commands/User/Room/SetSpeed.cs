namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class SetSpeed : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length < 2)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("input.intonly", session.Langue));
            return;
        }

        if (int.TryParse(Params[1], out var setSpeedCount))
        {
            Room.GetRoomItemHandler().SetSpeed(setSpeedCount);
        }
        else
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("input.intonly", session.Langue));
        }
    }
}
