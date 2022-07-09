using Wibbo.Communication.Packets.Outgoing.Help;
using Wibbo.Game.Clients;

namespace Wibbo.Communication.Packets.Incoming.Guide
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