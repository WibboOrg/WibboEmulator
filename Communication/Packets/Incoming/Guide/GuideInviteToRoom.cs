using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.GameClients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GuideInviteToRoom : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            GameClient requester = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.GetHabbo().GuideOtherUserId);
            if (requester == null)
            {
                return;
            }

            Room room = Session.GetHabbo().CurrentRoom;
            ServerPacket message = new ServerPacket(ServerPacketHeader.OnGuideSessionInvitedToGuideRoom);

            if (room == null)
            {
                message.WriteInteger(0);
                message.WriteString("");
            }
            else
            {
                message.WriteInteger(room.Id);
                message.WriteString(room.RoomData.Name);
            }

            requester.SendPacket(message);
            Session.SendPacket(message);
        }
    }
}