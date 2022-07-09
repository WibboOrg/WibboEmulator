using Wibbo.Game.Rooms;

namespace Wibbo.Communication.RCON.Commands.Hotel
{
    internal class UnloadCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            if (parameters.Length != 2)
            {
                return false;
            }


            if (!int.TryParse(parameters[1], out int RoomId))
            {
                return false;
            }

            if (RoomId == 0)
            {
                return false;
            }

            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(RoomId);
            if (room == null)
            {
                return false;
            }

            WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(room);
            return true;
        }
    }
}
