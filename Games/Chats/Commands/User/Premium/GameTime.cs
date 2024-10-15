namespace WibboEmulator.Games.Chats.Commands.User.Premium;

using WibboEmulator.Games.Animations;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class GameTime : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (AnimationManager.IsActivate)
        {
            var time = AnimationManager.Time;
            Session.SendWhisper("Prochaine animation de Jack & Daisy dans " + time);
        }
        else
        {
            Session.SendWhisper("Les animations de Jack & Daisy sont désactivées.");
        }
    }
}
