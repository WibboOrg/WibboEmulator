using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class StartGameJD : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            int roomId = 0;
            if (Params.Length > 1)
                int.TryParse(Params[1], out roomId);

            WibboEnvironment.GetGame().GetAnimationManager().StartGame(roomId);
            Session.SendWhisper("Lancement de l'animation de Jack & Daisy !");
        }
    }
}
