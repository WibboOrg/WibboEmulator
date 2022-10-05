namespace WibboEmulator.Communication.Packets.Incoming.Guide;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;

internal class OnGuideEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var userId = Packet.PopInt();
        var message = Packet.PopString();

        var guideManager = WibboEnvironment.GetGame().GetHelpManager();
        if (guideManager.GuidesCount <= 0)
        {
            session.SendPacket(new OnGuideSessionErrorComposer(2));
            return;
        }

        if (session.GetUser().OnDuty)
        {
            guideManager.RemoveGuide(session.GetUser().Id);
        }

        var guideId = guideManager.GetRandomGuide();
        if (guideId == 0)
        {
            session.SendPacket(new OnGuideSessionErrorComposer(2));
            return;
        }

        var guide = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(guideId);

        session.SendPacket(new OnGuideSessionAttachedComposer(false, userId, message, 30));
        guide.SendPacket(new OnGuideSessionAttachedComposer(true, userId, message, 15));

        guide.GetUser().GuideOtherUserId = session.GetUser().Id;
        session.GetUser().GuideOtherUserId = guide.GetUser().Id;
    }
}
