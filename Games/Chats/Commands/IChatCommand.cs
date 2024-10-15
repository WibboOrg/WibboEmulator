namespace WibboEmulator.Games.Chats.Commands;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

public interface IChatCommand
{
    void Execute(GameClient Session, Room room, RoomUser roomUser, string[] parameters);
}
