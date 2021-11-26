using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class RegenMaps : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room.GetGameMap().GenerateMaps();
            UserRoom.SendWhisperChat("Rafraichissement de la map d'appartement");
        }
    }
}
