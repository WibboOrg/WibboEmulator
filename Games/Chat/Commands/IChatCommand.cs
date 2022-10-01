using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands
{
    public interface IChatCommand
    {
        void Execute(Client session, Room room, RoomUser roomUser, string[] parts);
    }
}
