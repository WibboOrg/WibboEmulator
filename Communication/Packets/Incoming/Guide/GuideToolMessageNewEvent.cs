namespace WibboEmulator.Communication.Packets.Incoming.Guide;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Games.GameClients;

internal sealed class GuideToolMessageNewEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var message = packet.PopString();

        var requester = GameClientManager.GetClientByUserID(Session.User.GuideOtherUserId);
        if (requester == null)
        {
            return;
        }

        if (Session.User.CheckChatMessage(message, "<GUIDEMESSAGE>"))
        {
            return;
        }

        requester.SendPacket(new OnGuideSessionMsgComposer(message, Session.User.Id));
        Session.SendPacket(new OnGuideSessionMsgComposer(message, Session.User.Id));
    }
}