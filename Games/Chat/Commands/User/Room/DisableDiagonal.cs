using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class DisableDiagonal : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room.GetGameMap().DiagonalEnabled = !Room.GetGameMap().DiagonalEnabled;
        }
    }
}
