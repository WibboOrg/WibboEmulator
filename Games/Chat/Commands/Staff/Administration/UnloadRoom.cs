using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class UnloadRoom : IChatCommand
    {
        public void Execute(Client session, Room room, RoomUser userRoom, string[] parameters)
        {
            if (parameters.Length < 2)
                return;

            if (!int.TryParse(parameters[1], out int roomId))
                return;

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(roomId, out Room roomTarget))
                return;

            WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(roomTarget);
        }
    }
}
