using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class UnloadRoom : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(int.Parse(Params[1]));
            if (room == null)
            {
                return;
            }

            WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(room);
        }
    }
}
