using Butterfly.Communication.Packets.Outgoing.Help;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class OnGuideSessionDetachedEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            bool state = Packet.PopBoolean();

            Client requester = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.GetHabbo().GuideOtherUserId);

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

            requester.SendPacket(new OnGuideSessionStartedComposer(Session.GetHabbo(), requester.GetHabbo()));
            Session.SendPacket(new OnGuideSessionStartedComposer(Session.GetHabbo(), requester.GetHabbo()));
        }
    }
}
