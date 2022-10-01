using WibboEmulator.Communication.Packets.Outgoing.Navigator;

using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.RCON.Commands.User
{
    internal class FollowCommand : IRCONCommand
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

            GameClient Client = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);
            if (Client == null)
            {
                return false;
            }


            if (!int.TryParse(parameters[2], out int Userid2))
            {
                return false;
            }

            if (Userid2 == 0)
            {
                return false;
            }

            GameClient Client2 = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid2);
            if (Client2 == null)
            {
                return false;
            }

            if (Client2.GetUser() == null || Client2.GetUser().CurrentRoom == null)
            {
                return false;
            }

            Room room = Client2.GetUser().CurrentRoom;
            if (room == null)
            {
                return false;
            }

            Client.SendPacket(new GetGuestRoomResultComposer(Client, room.RoomData, false, true));
            return true;
        }
    }
}
