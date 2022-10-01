using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands
{
    public interface IChatCommand
    {
        void Execute(Client session, Room room, RoomUser roomUser, string[] parts);
    }
}
