using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class LetUserInEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetUser() == null)
            {
                return;
            }

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(Session))
            {
                return;
            }

            string username = Packet.PopString();
            bool allowUserToEnter = Packet.PopBoolean();

            Client clientByUsername = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(username);
            if (clientByUsername == null || clientByUsername.GetUser() == null)
            {
                return;
            }

            if (clientByUsername.GetUser().LoadingRoomId != room.Id)
            {
                return;
            }

            RoomUser user = room.GetRoomUserManager().GetRoomUserByUserId(clientByUsername.GetUser().Id);

            if (user != null)
            {
                return;
            }

            if (allowUserToEnter)
            {
                clientByUsername.SendPacket(new FlatAccessibleComposer(""));

                clientByUsername.GetUser().AllowDoorBell = true;

                if (!clientByUsername.GetUser().EnterRoom(Session.GetUser().CurrentRoom))
                {
                    clientByUsername.SendPacket(new CloseConnectionComposer());
                }
            }
            else
            {
                clientByUsername.SendPacket(new FlatAccessDeniedComposer(""));
            }
        }
    }
}