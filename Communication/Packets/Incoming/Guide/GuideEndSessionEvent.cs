using Butterfly.Communication.Packets.Outgoing.Help;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Guide
{
    internal class GuideEndSessionEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Client requester = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.GetUser().GuideOtherUserId);

            Session.SendPacket(new OnGuideSessionEndedComposer(1));

            Session.GetUser().GuideOtherUserId = 0;
            if (Session.GetUser().OnDuty)
            {
                ButterflyEnvironment.GetGame().GetHelpManager().EndService(Session.GetUser().Id);
            }

            if (requester != null)
            {
                requester.SendPacket(new OnGuideSessionEndedComposer(1));
                requester.GetUser().GuideOtherUserId = 0;

                if (requester.GetUser().OnDuty)
                {
                    ButterflyEnvironment.GetGame().GetHelpManager().EndService(requester.GetUser().Id);
                }
            }
        }
    }
}