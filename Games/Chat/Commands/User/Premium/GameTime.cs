namespace WibboEmulator.Games.Chat.Commands.User.Premium;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class GameTime : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (WibboEnvironment.GetGame().GetAnimationManager().IsActivate())
        {
            var Time = WibboEnvironment.GetGame().GetAnimationManager().GetTime();
            session.SendWhisper("Prochaine animation de Jack & Daisy dans " + Time);
        }
        else
        {
            session.SendWhisper("Les animations de Jack & Daisy sont désactivées");
        }
    }
}
