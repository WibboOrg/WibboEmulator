using Butterfly.Game.GameClients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class RegenMaps : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room.GetGameMap().GenerateMaps();
            UserRoom.SendWhisperChat("Rafraichissement de la map d'appartement");
        }
    }
}
