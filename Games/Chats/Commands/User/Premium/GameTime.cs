namespace WibboEmulator.Games.Chats.Commands.User.Premium;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class GameTime : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (WibboEnvironment.GetGame().GetAnimationManager().IsActivate())
        {
            var time = WibboEnvironment.GetGame().GetAnimationManager().GetTime();
            session.SendWhisper("Prochaine animation de Jack & Daisy dans " + time);
        }
        else
        {
            session.SendWhisper("Les animations de Jack & Daisy sont désactivées");
        }
    }
}
