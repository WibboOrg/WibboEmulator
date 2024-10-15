namespace WibboEmulator.Communication.Packets.Incoming.Guide;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Helps;

internal sealed class GuideEndSessionEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var requester = GameClientManager.GetClientByUserID(Session.User.GuideOtherUserId);

        Session.SendPacket(new OnGuideSessionEndedComposer(1));

        Session.User.GuideOtherUserId = 0;
        if (Session.User.OnDuty)
        {
            HelpManager.MarkAsOffDuty(Session.User.Id);
        }

        if (requester != null)
        {
            requester.SendPacket(new OnGuideSessionEndedComposer(1));
            requester.User.GuideOtherUserId = 0;

            if (requester.User.OnDuty)
            {
                HelpManager.MarkAsOffDuty(requester.User.Id);
            }
        }
    }
}
