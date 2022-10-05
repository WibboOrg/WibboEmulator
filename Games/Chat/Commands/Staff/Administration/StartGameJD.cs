namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class StartGameJD : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        var roomId = 0;
        if (Params.Length > 1)
        {
            int.TryParse(Params[1], out roomId);
        }

        WibboEnvironment.GetGame().GetAnimationManager().StartGame(roomId);
        session.SendWhisper("Lancement de l'animation de Jack & Daisy !");
    }
}
