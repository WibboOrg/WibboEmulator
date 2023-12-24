namespace WibboEmulator.Communication.Packets.Incoming.Guide;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;

internal sealed class GuideToolMessageNewEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var message = packet.PopString();

        var requester = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(session.User.GuideOtherUserId);
        if (requester == null)
        {
            return;
        }

        if (session.User.CheckChatMessage(message, "<GUIDEMESSAGE>"))
        {
            return;
        }

        requester.SendPacket(new OnGuideSessionMsgComposer(message, session.User.Id));
        session.SendPacket(new OnGuideSessionMsgComposer(message, session.User.Id));
    }
}