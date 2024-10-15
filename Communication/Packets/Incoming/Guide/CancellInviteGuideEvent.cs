namespace WibboEmulator.Communication.Packets.Incoming.Guide;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;

internal sealed class CancellInviteGuideEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var requester = GameClientManager.GetClientByUserID(Session.User.GuideOtherUserId);

        Session.SendPacket(new OnGuideSessionDetachedComposer());

        requester?.SendPacket(new OnGuideSessionDetachedComposer());
    }
}