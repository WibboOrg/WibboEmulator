using WibboEmulator.Communication.Packets.Outgoing.Navigator;

using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

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


            if (!int.TryParse(parameters[2], out int RoomId))
            {
                return false;
            }

            if (RoomId == 0)
            {
                return false;
            }

            Room room = WibboEnvironment.GetGame().GetRoomManager().LoadRoom(RoomId);
            if (room == null)
            {
                return false;
            }

            Client.SendPacket(new GetGuestRoomResultComposer(Client, room.RoomData, false, true));
            return true;
        }
    }
}
