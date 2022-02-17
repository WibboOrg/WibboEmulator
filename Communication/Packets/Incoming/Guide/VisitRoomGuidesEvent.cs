using Butterfly.Communication.Packets.Outgoing.Help;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class VisitRoomGuidesEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Client requester = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.GetHabbo().GuideOtherUserId);
            if (requester == null)
            {
                return;
            }

            Session.SendPacket(new OnGuideSessionRequesterRoomComposer(requester.GetHabbo().CurrentRoomId));
        }
    }
}
