using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Incoming.Guide
{
    internal class CancellInviteGuideEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            GameClient requester = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(Session.GetUser().GuideOtherUserId);

            Session.SendPacket(new OnGuideSessionDetachedComposer());

            if (requester != null)
            {
                requester.SendPacket(new OnGuideSessionDetachedComposer());
            }
        }
    }
}