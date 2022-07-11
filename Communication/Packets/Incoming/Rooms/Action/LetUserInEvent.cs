using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

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

            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null || !room.CheckRights(Session))
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