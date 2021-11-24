using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GuideToolMessageNewEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            string message = Packet.PopString();

            Client requester = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.GetHabbo().GuideOtherUserId);
            if (requester == null)
            {
                return;
            }

            if (Session.Antipub(message, "<GUIDEMESSAGE>"))
            {
                return;
            }

            ServerPacket messageC = new ServerPacket(ServerPacketHeader.OnGuideSessionMsg);
            messageC.WriteString(message);
            messageC.WriteInteger(Session.GetHabbo().Id);

            requester.SendPacket(messageC);
            Session.SendPacket(messageC);
        }
    }
}