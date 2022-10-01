using WibboEmulator.Communication.Packets.Outgoing.Navigator;

using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.RCON.Commands.User
{
    internal class SendUserCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            if (parameters.Length != 3)
            {
                return false;
            }

            if (!int.TryParse(parameters[1], out int Userid))
            {
                return false;
            }

            if (Userid == 0)
            {
                return false;
            }

            Client Client = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);
            if (Client == null || Client.GetUser() == null)
            {
                return false;
            }


            if (!int.TryParse(parameters[2], out int roomId))
            {
                return false;
            }

            if (roomId == 0)
            {
                return false;
            }

            RoomData roomData = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomId);
            if (roomData == null)
            {
                return false;
            }

            Client.SendPacket(new GetGuestRoomResultComposer(Client, roomData, false, true));
            return true;
        }
    }
}
