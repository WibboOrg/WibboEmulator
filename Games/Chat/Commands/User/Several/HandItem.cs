using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms.Games;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class HandItem : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
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
