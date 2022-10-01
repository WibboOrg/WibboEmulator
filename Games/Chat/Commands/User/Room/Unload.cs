using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class Unload : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(Session.GetUser().CurrentRoom);
        }
    }
}
