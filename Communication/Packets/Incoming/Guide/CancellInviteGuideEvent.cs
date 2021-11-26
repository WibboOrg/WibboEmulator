using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class CancellInviteGuideEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            Client requester = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.GetHabbo().GuideOtherUserId);

            Session.SendPacket(new OnGuideSessionDetachedComposer());

            if (requester != null)
            {
                requester.SendPacket(new OnGuideSessionDetachedComposer());
            }
        }
    }
}