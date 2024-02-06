namespace WibboEmulator.Communication.Packets.Incoming.Guide;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;

internal sealed class OnGuideEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var userId = packet.PopInt();
        var message = packet.PopString();

        var guideManager = WibboEnvironment.GetGame().GetHelpManager();
        if (guideManager.Count <= 0)
        {
            session.SendPacket(new OnGuideSessionErrorComposer(2));
            return;
        }

        if (session.User.OnDuty)
        {
            guideManager.TryRemoveGuide(session.User.Id);
        }

        var guideId = guideManager.RandomAvailableGuide();
        if (guideId == 0)
        {
            session.SendPacket(new OnGuideSessionErrorComposer(2));
            return;
        }

        guideManager.GuideLeftService(guideId);

        var guide = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(guideId);

        session.SendPacket(new OnGuideSessionAttachedComposer(false, userId, message, 30));
        guide.SendPacket(new OnGuideSessionAttachedComposer(true, userId, message, 15));

        guide.User.GuideOtherUserId = session.User.Id;
        session.User.GuideOtherUserId = guide.User.Id;
    }
}
