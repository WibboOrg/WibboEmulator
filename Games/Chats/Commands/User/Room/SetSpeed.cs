namespace WibboEmulator.Games.Chats.Commands.User.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class SetSpeed : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("input.intonly", session.Langue));
            return;
        }

        if (int.TryParse(parameters[1], out var setSpeedCount))
        {
            room.RoomItemHandling.SetSpeed(setSpeedCount);
        }
        else
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("input.intonly", session.Langue));
        }
    }
}
