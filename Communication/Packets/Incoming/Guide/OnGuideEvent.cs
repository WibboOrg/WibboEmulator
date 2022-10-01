using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Help;

namespace WibboEmulator.Communication.Packets.Incoming.Guide
{
    internal class OnGuideEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int userId = Packet.PopInt();
            string message = Packet.PopString();

            HelpManager guideManager = WibboEnvironment.GetGame().GetHelpManager();
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

            GameClient guide = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(guideId);

            Session.SendPacket(new OnGuideSessionAttachedComposer(false, userId, message, 30));
            guide.SendPacket(new OnGuideSessionAttachedComposer(true, userId, message, 15));

            guide.GetUser().GuideOtherUserId = Session.GetUser().Id;
            Session.GetUser().GuideOtherUserId = guide.GetUser().Id;
        }
    }
}
