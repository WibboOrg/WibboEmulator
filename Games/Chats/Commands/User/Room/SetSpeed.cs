namespace WibboEmulator.Games.Chats.Commands.User.Room;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class SetSpeed : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            Session.SendWhisper(LanguageManager.TryGetValue("input.intonly", Session.Language));
            return;
        }

        if (int.TryParse(parameters[1], out var setSpeedCount))
        {
            room.RoomItemHandling.SetSpeed(setSpeedCount);
        }
        else
        {
            Session.SendWhisper(LanguageManager.TryGetValue("input.intonly", Session.Language));
        }
    }
}
