using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.RCON.Commands.Hotel
{
    internal class UnloadCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            if (parameters.Length != 2)
            {
                return false;
            }


            if (!int.TryParse(parameters[1], out int roomId))
            {
                return false;
            }

            if (roomId == 0)
            {
                return false;
            }

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(roomId, out Room room))
                return false;

            WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(room);
            return true;
        }
    }
}
