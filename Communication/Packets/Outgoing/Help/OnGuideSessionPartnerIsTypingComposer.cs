namespace Wibbo.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionPartnerIsTypingComposer : ServerPacket
    {
        public OnGuideSessionPartnerIsTypingComposer()
            : base(ServerPacketHeader.GUIDE_SESSION_PARTNER_IS_TYPING)
        {

        }
    }
}
