namespace WibboEmulator.Games.Chats.Commands.User.Premium;

using WibboEmulator.Games.Animations;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class GameTime : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (AnimationManager.IsActivate)
        {
            var time = AnimationManager.Time;
            session.SendWhisper("Prochaine animation de Jack & Daisy dans " + time);
        }
        else
        {
            session.SendWhisper("Les animations de Jack & Daisy sont désactivées.");
        }
    }
}
