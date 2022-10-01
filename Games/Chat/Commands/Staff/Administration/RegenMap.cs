using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class RegenMap : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room.GetGameMap().GenerateMaps();
            Session.SendWhisper("Rafraichissement de la map d'appartement");
        }
    }
}
