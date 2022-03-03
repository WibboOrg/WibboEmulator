using Butterfly.Communication.Packets.Outgoing.Help;
using Butterfly.Game.Clients;
using Butterfly.Game.Help;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class OnGuideEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int userId = Packet.PopInt();
            string message = Packet.PopString();

            HelpManager guideManager = ButterflyEnvironment.GetGame().GetHelpManager();
            if (guideManager.GuidesCount <= 0)
            {
                Session.SendPacket(new OnGuideSessionErrorComposer(2));
                return;
            }

            if (Session.GetUser().OnDuty == true)
            {
                guideManager.RemoveGuide(Session.GetUser().Id);
            }

            int guideId = guideManager.GetRandomGuide();
            if (guideId == 0)
            {
                Session.SendPacket(new OnGuideSessionErrorComposer(2));
                return;
            }

            Client guide = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(guideId);

            Session.SendPacket(new OnGuideSessionAttachedComposer(false, userId, message, 30));
            guide.SendPacket(new OnGuideSessionAttachedComposer(true, userId, message, 15));

            guide.GetUser().GuideOtherUserId = Session.GetUser().Id;
            Session.GetUser().GuideOtherUserId = guide.GetUser().Id;
        }
    }
}
