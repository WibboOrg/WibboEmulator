using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands
{
    public interface IChatCommand
    {
        void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params);
    }
}
