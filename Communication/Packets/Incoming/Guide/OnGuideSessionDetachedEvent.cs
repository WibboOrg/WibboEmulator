namespace WibboEmulator.Communication.Packets.Incoming.Guide;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;

internal sealed class OnGuideSessionDetachedEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var state = packet.PopBoolean();

        var requester = GameClientManager.GetClientByUserID(Session.User.GuideOtherUserId);

        if (!state)
        {
            Session.SendPacket(new OnGuideSessionDetachedComposer());

            if (requester == null)
            {
                return;
            }

            requester.SendPacket(new OnGuideSessionErrorComposer(1));
            return;
        }

        if (requester == null)
        {
            return;
        }

        requester.SendPacket(new OnGuideSessionStartedComposer(Session.User, requester.User));
        Session.SendPacket(new OnGuideSessionStartedComposer(Session.User, requester.User));
    }
}
