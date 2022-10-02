using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Incoming.Guide
{
    internal class OnGuideSessionDetachedEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            bool state = Packet.PopBoolean();

            GameClient requester = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(Session.GetUser().GuideOtherUserId);

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
