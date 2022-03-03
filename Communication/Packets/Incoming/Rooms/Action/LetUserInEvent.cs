using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Communication.Packets.Outgoing.Rooms.Session;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
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

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null || !room.CheckRights(Session))
            {
                return;
            }

            string username = Packet.PopString();
            bool allowUserToEnter = Packet.PopBoolean();

            Client clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(username);
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