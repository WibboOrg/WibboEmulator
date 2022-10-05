using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class Unload : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params) => WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(Session.GetUser().CurrentRoom);
    }
}
