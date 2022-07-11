
using WibboEmulator.Communication.Packets.Outgoing.Notifications.NotifCustom;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
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
