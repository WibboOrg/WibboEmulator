using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class StartGameJD : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            ButterflyEnvironment.GetGame().GetAnimationManager().StartGame();
            UserRoom.SendWhisperChat("Lancement de l'animation de Jack & Daisy !");
            ButterflyEnvironment.GetGame().GetClientManager().SendMessage(new NotifTopComposer("Petite animation à l'improviste ! (Jack & Daisy)"));
        }
    }
}
