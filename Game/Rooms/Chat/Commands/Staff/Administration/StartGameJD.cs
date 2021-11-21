using Butterfly.Game.GameClients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class StartGameJD : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            ButterflyEnvironment.GetGame().GetAnimationManager().StartGame();
            UserRoom.SendWhisperChat("Lancement de l'animation de Jack & Daisy !");
        }
    }
}
