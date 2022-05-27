using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
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
