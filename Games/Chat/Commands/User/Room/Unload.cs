using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class Unload : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(Session.GetUser().CurrentRoom);
        }
    }
}
