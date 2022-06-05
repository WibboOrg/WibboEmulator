using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class DisableOblique : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room currentRoom = Session.GetUser().CurrentRoom;
            currentRoom.GetGameMap().ObliqueDisable = !currentRoom.GetGameMap().ObliqueDisable;

        }
    }
}
