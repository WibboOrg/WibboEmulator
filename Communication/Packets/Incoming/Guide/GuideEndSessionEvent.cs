namespace WibboEmulator.Communication.Packets.Incoming.Guide;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;

internal class GuideEndSessionEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var requester = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(session.GetUser().GuideOtherUserId);

        session.SendPacket(new OnGuideSessionEndedComposer(1));

        session.GetUser().GuideOtherUserId = 0;
        if (session.GetUser().OnDuty)
        {
            WibboEnvironment.GetGame().GetHelpManager().EndService(session.GetUser().Id);
        }

        if (requester != null)
        {
            requester.SendPacket(new OnGuideSessionEndedComposer(1));
            requester.GetUser().GuideOtherUserId = 0;

            if (requester.GetUser().OnDuty)
            {
                WibboEnvironment.GetGame().GetHelpManager().EndService(requester.GetUser().Id);
            }
        }
    }
}