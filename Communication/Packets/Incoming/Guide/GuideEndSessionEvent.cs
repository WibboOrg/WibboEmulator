using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Help;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GuideEndSessionEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            Client requester = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.GetHabbo().GuideOtherUserId);

            Session.SendPacket(new OnGuideSessionEndedComposer(1));

            Session.GetHabbo().GuideOtherUserId = 0;
            if (Session.GetHabbo().OnDuty)
            {
                ButterflyEnvironment.GetGame().GetHelpManager().EndService(Session.GetHabbo().Id);
            }

            if (requester != null)
            {
                requester.SendPacket(new OnGuideSessionEndedComposer(1));
                requester.GetHabbo().GuideOtherUserId = 0;

                if (requester.GetHabbo().OnDuty)
                {
                    ButterflyEnvironment.GetGame().GetHelpManager().EndService(requester.GetHabbo().Id);
                }
            }
        }
    }
}