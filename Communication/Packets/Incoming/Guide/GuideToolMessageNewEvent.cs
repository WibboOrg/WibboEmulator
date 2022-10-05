namespace WibboEmulator.Communication.Packets.Incoming.Guide;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;

internal class GuideToolMessageNewEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var message = Packet.PopString();

        var requester = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(session.GetUser().GuideOtherUserId);
        if (requester == null)
        {
            return;
        }

        if (session.Antipub(message, "<GUIDEMESSAGE>"))
        {
            return;
        }

        requester.SendPacket(new OnGuideSessionMsgComposer(message, session.GetUser().Id));
        session.SendPacket(new OnGuideSessionMsgComposer(message, session.GetUser().Id));
    }
}