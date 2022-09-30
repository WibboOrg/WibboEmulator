
using WibboEmulator.Communication.Packets.Outgoing.Notifications.NotifCustom;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class StartGameJD : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            int roomId = 0;
            if (Params.Length > 1)
                int.TryParse(Params[1], out roomId);

            WibboEnvironment.GetGame().GetAnimationManager().StartGame(roomId);
            Session.SendWhisper("Lancement de l'animation de Jack & Daisy !");
        }
    }
}
