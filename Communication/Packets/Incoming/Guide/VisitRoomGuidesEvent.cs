using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class VisitRoomGuidesEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            Client requester = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.GetHabbo().GuideOtherUserId);
            if (requester == null)
            {
                return;
            }

            int roomid = requester.GetHabbo().CurrentRoomId;

            ServerPacket message = new ServerPacket(ServerPacketHeader.OnGuideSessionRequesterRoom);
            message.WriteInteger(roomid);
            Session.SendPacket(message);
        }
    }
}
