using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands
{
    public interface IChatCommand
    {
        void Execute(GameClient session, Room room, RoomUser roomUser, string[] parts);
    }
}
