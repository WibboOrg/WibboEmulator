namespace WibboEmulator.Communication.Packets.Incoming.Guide;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Helps;

internal sealed class OnGuideEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var userId = packet.PopInt();
        var message = packet.PopString();

        if (HelpManager.Count <= 0)
        {
            Session.SendPacket(new OnGuideSessionErrorComposer(2));
            return;
        }

        if (Session.User.OnDuty)
        {
            HelpManager.TryRemoveGuide(Session.User.Id);
        }

        var guideId = HelpManager.RandomAvailableGuide();
        if (guideId == 0)
        {
            Session.SendPacket(new OnGuideSessionErrorComposer(2));
            return;
        }

        HelpManager.GuideLeftService(guideId);

        var guide = GameClientManager.GetClientByUserID(guideId);

        Session.SendPacket(new OnGuideSessionAttachedComposer(false, userId, message, 30));
        guide.SendPacket(new OnGuideSessionAttachedComposer(true, userId, message, 15));

        guide.User.GuideOtherUserId = Session.User.Id;
        Session.User.GuideOtherUserId = guide.User.Id;
    }
}
