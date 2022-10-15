namespace WibboEmulator.Communication.Packets.Incoming.Guide;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;

internal class GuideEndSessionEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var requester = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(session.User.GuideOtherUserId);

        session.SendPacket(new OnGuideSessionEndedComposer(1));

        session.
        User.GuideOtherUserId = 0;
        if (session.User.OnDuty)
        {
            WibboEnvironment.GetGame().GetHelpManager().EndService(session.User.Id);
        }

        if (requester != null)
        {
            requester.SendPacket(new OnGuideSessionEndedComposer(1));
            requester.User.GuideOtherUserId = 0;

            if (requester.User.OnDuty)
            {
                WibboEnvironment.GetGame().GetHelpManager().EndService(requester.User.Id);
            }
        }
    }
}