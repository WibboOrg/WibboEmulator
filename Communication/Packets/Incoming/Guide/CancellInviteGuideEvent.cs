namespace WibboEmulator.Communication.Packets.Incoming.Guide;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;

internal class CancellInviteGuideEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var requester = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(session.User.GuideOtherUserId);

        session.SendPacket(new OnGuideSessionDetachedComposer());

        if (requester != null)
        {
            requester.SendPacket(new OnGuideSessionDetachedComposer());
        }
    }
}