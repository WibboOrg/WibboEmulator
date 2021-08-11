using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Rooms.Session;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class LetUserInEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
            {
                return;
            }

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || !room.CheckRights(Session))
            {
                return;
            }

            string username = Packet.PopString();
            bool allowUserToEnter = Packet.PopBoolean();

            GameClient clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(username);
            if (clientByUsername == null || clientByUsername.GetHabbo() == null)
            {
                return;
            }

            if (clientByUsername.GetHabbo().LoadingRoomId != room.Id)
            {
                return;
            }

            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabboId(clientByUsername.GetHabbo().Id);

            if (user != null)
            {
                return;
            }

            if (allowUserToEnter)
            {
                ServerPacket Response = new ServerPacket(ServerPacketHeader.ROOM_DOORBELL_CLOSE);
                Response.WriteString("");
                clientByUsername.SendPacket(Response);

                clientByUsername.GetHabbo().AllowDoorBell = true;

                if (!clientByUsername.GetHabbo().EnterRoom(Session.GetHabbo().CurrentRoom))
                {
                    clientByUsername.SendPacket(new CloseConnectionComposer());
                }
            }
            else
            {
                ServerPacket Response = new ServerPacket(ServerPacketHeader.ROOM_DOORBELL_DENIED);
                Response.WriteString("");
                clientByUsername.SendPacket(Response);
            }
        }
    }
}