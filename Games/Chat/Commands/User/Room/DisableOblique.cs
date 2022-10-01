using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class DisableOblique : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room.GetGameMap().ObliqueDisable = !Room.GetGameMap().ObliqueDisable;
        }
    }
}
