namespace WibboEmulator.Communication.Packets.Outgoing.Help;

internal sealed class OnGuideSessionPartnerIsTypingComposer : ServerPacket
{
    public OnGuideSessionPartnerIsTypingComposer()
        : base(ServerPacketHeader.GUIDE_SESSION_PARTNER_IS_TYPING)
    {

    }
}
