using Wibbo.Game.Clients;
using Wibbo.Game.Rooms.Games;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class HandItem : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
            {
                return;
            }

            int handitemid;
            int.TryParse(Params[1], out handitemid);
            if (handitemid < 0)
            {
                return;
            }

            UserRoom.CarryItem(handitemid);

        }
    }
}
