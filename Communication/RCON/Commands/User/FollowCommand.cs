using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.RCON.Commands.User
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

            GameClient Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);
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

            GameClient Client2 = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid2);
            if (Client2 == null)
            {
                return false;
            }

            if (Client2.GetHabbo() == null || Client2.GetHabbo().CurrentRoom == null)
            {
                return false;
            }

            Room room = Client2.GetHabbo().CurrentRoom;
            if (room == null)
            {
                return false;
            }

            Client.SendPacket(new GetGuestRoomResultComposer(Client, room.RoomData, false, true));
            return true;
        }
    }
}
