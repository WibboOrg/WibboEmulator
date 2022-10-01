using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Guide
{
    internal class CancellInviteGuideEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Client requester = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.GetUser().GuideOtherUserId);

            Session.SendPacket(new OnGuideSessionDetachedComposer());

            if (requester != null)
            {
                requester.SendPacket(new OnGuideSessionDetachedComposer());
            }
        }
    }
}