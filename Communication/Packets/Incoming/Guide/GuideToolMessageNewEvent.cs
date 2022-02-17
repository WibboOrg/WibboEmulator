using Butterfly.Communication.Packets.Outgoing.Help;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GuideToolMessageNewEvent : IPacketEvent
    {
        public double Delay => 250;

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

            requester.SendPacket(new OnGuideSessionMsgComposer(message, Session.GetHabbo().Id));
            Session.SendPacket(new OnGuideSessionMsgComposer(message, Session.GetHabbo().Id));
        }
    }
}