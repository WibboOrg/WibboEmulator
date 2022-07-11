using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Game.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Guide
{
    internal class OnGuideSessionDetachedEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            bool state = Packet.PopBoolean();

            Client requester = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.GetUser().GuideOtherUserId);

            if (!state)
            {
                Session.SendPacket(new OnGuideSessionDetachedComposer());

                if (requester == null)
                {
                    return;
                }

                requester.SendPacket(new OnGuideSessionErrorComposer(1));
                return;
            }

            if (requester == null)
            {
                return;
            }

            requester.SendPacket(new OnGuideSessionStartedComposer(Session.GetUser(), requester.GetUser()));
            Session.SendPacket(new OnGuideSessionStartedComposer(Session.GetUser(), requester.GetUser()));
        }
    }
}
