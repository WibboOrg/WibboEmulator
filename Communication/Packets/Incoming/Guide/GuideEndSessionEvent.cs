using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Guide
{
    internal class GuideEndSessionEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Client requester = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.GetUser().GuideOtherUserId);

            Session.SendPacket(new OnGuideSessionEndedComposer(1));

            Session.GetUser().GuideOtherUserId = 0;
            if (Session.GetUser().OnDuty)
            {
                WibboEnvironment.GetGame().GetHelpManager().EndService(Session.GetUser().Id);
            }

            if (requester != null)
            {
                requester.SendPacket(new OnGuideSessionEndedComposer(1));
                requester.GetUser().GuideOtherUserId = 0;

                if (requester.GetUser().OnDuty)
                {
                    WibboEnvironment.GetGame().GetHelpManager().EndService(requester.GetUser().Id);
                }
            }
        }
    }
}