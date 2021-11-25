using Butterfly.Game.Clients;
using Butterfly.Game.Rooms.Games;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class HandItem : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            if (UserRoom.Team != TeamType.none || UserRoom.InGame)
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
