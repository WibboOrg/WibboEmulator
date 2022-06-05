
using Wibbo.Communication.Packets.Outgoing.Notifications.NotifCustom;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class StartGameJD : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            WibboEnvironment.GetGame().GetAnimationManager().StartGame();
            Session.SendWhisper("Lancement de l'animation de Jack & Daisy !");
            WibboEnvironment.GetGame().GetClientManager().SendMessage(new NotifTopComposer("Petite animation à l'improviste ! (Jack & Daisy)"));
        }
    }
}
