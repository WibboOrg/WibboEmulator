namespace WibboEmulator.Communication.Packets.Incoming.Guide;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;

internal class OnGuideSessionDetachedEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var state = packet.PopBoolean();

        var requester = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(session.GetUser().GuideOtherUserId);

        if (!state)
        {
            session.SendPacket(new OnGuideSessionDetachedComposer());

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

        requester.SendPacket(new OnGuideSessionStartedComposer(session.GetUser(), requester.GetUser()));
        session.SendPacket(new OnGuideSessionStartedComposer(session.GetUser(), requester.GetUser()));
    }
}
